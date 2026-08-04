using BrestCanser.Api.Authentication;
using BrestCanser.Api.Authentication.Filter;
using BrestCanser.Api.Engine;
using BrestCanser.Api.Options;
using BrestCanser.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace BrestCanser.Api;

public static class DependencyInjection
{
	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration Configuration)
	{
		services.AddControllers().AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		services.AddMapsterConfig()
				.AddFluentValidatonConfig()
				.AddAuthorConfig(Configuration);


		services.AddSignalR();


		//add ConnectionString and register ApplicationDbContext
		var connectionString = Configuration.GetConnectionString("DefaultConnection") ??
			 throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

		services.AddDbContext<ApplicationDbContext>(options
			=> options.UseSqlServer(connectionString));


		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IEmailSender, EmailService>();
		services.AddScoped<IChatService, GeminiService>();
		services.AddScoped<IHistoryService, HistoryService>();
		services.AddScoped<IImageService, ImageService>();
		services.AddScoped<IMLService, MLService>();
		services.AddScoped<INotificationService, NotificationService>();
		services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
		services.AddScoped<RiskAssessmentEngine>();

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();

		services.AddRateLimitingConfig();

		services.Configure<MailSettings>(Configuration.GetSection(nameof(MailSettings)));

		services.Configure<CloudinarySettings>(Configuration.GetSection(nameof(CloudinarySettings)));

		services.Configure<RiskScoringOptions>(Configuration.GetSection(RiskScoringOptions.SectionName));



		return services;
	}

	private static IServiceCollection AddAuthorConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddIdentity<ApplicationUser, ApplicationRole>()
			 .AddEntityFrameworkStores<ApplicationDbContext>()
			 .AddDefaultTokenProviders();

		services.AddScoped<IJwtProvider, JwtProvider>();


        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


        services.AddOptions<JwtOptions>()
			.BindConfiguration(JwtOptions.SectionName)
			.ValidateDataAnnotations()
			.ValidateOnStart();

		var JwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(o =>
		{
			o.SaveToken = true;
			o.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.Key!)),
				ValidIssuer = JwtSettings?.Issuer,
				ValidAudience = JwtSettings?.Audience,
			};

			o.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];
					var path = context.HttpContext.Request.Path;

					if (!string.IsNullOrEmpty(accessToken) &&
						path.StartsWithSegments("/hubs"))
					{
						context.Token = accessToken;
					}

					return Task.CompletedTask;
				}
			};
		});

		services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequiredLength = 8;
			//options.SignIn.RequireConfirmedEmail = true;
			options.User.RequireUniqueEmail = true;
		});

		return services;
	}
	private static IServiceCollection AddFluentValidatonConfig(this IServiceCollection services)
	{
		services.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

		return services;
	}

	private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
	{
		var mapingconfig = TypeAdapterConfig.GlobalSettings;
		mapingconfig.Scan(Assembly.GetExecutingAssembly());
		services.AddSingleton<IMapper>(new Mapper(mapingconfig));

		return services;
	}
	private static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			static string GetClientKey(HttpContext context)
			{
				return context.User.Identity?.IsAuthenticated == true
					? context.User.GetUserId()!
					: context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
			}

			// 30 requests per minute with queuing
			options.AddPolicy(RateLimiters.GeneralPolicy, context =>
				RateLimitPartition.GetSlidingWindowLimiter(
					GetClientKey(context),
					_ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 30,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 6,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 5
					}
				)
			);

			// 10 requests per minute without queuing
			options.AddPolicy(RateLimiters.AuthPolicy, context =>
				RateLimitPartition.GetSlidingWindowLimiter(
					GetClientKey(context),
					_ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 10,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 6,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0
					}
				)
			);

			// 5 requests per minute without queuing
			options.AddPolicy(RateLimiters.SensitivePolicy, context =>
				RateLimitPartition.GetSlidingWindowLimiter(
					GetClientKey(context),
					_ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 3,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 6,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0
					}
				)
			);

			// Custom Response
			options.OnRejected = async (context, cancellationToken) =>
			{
				context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

				if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
					context.HttpContext.Response.Headers.RetryAfter =
						((int)retryAfter.TotalSeconds).ToString();

				await context.HttpContext.Response.WriteAsJsonAsync(new
				{
					type = "https://tools.ietf.org/html/rfc6585#section-4",
					title = "Too Many Requests",
					status = 429,
					detail = "You have exceeded the rate limit. Please try again later."
				}, cancellationToken);
			};
		});

		return services;
	}
}