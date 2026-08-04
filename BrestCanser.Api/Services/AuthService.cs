using BrestCanser.Api.Authentication;
using BrestCanser.Api.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace BrestCanser.Api.Services;

public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IJwtProvider _jwtProvider;
	private readonly ApplicationDbContext _context;
	private readonly ILogger<AuthService> _logger;
	private readonly IEmailSender _emailSender;
	private readonly int _refreshTokenExpiryDays = 14;

	public AuthService(UserManager<ApplicationUser> userManager,
		IJwtProvider jwtProvider,
		ApplicationDbContext context,
		ILogger<AuthService> logger,
		IEmailSender emailSender)
	{
		_userManager = userManager;
		_jwtProvider = jwtProvider;
		_context = context;
		_logger = logger;
		_emailSender = emailSender;
	}

    public async Task<Result<AuthorResponse>> GetTokenAsync(
    string email,
    string password,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
            return Result.Failure<AuthorResponse>(UserErrors.InvalidCredentials);

        var (userRoles, Permission) = await GetRolesAndPermissions(user, cancellationToken);

        user.RefreshTokens.RemoveAll(x =>
            !x.IsActive || x.ExpiresOn <= DateTime.UtcNow);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user,userRoles,Permission);

        var (refreshToken, refreshTokenExpiration) =
            await CreateRefreshTokenAsync(user);

        var response = new AuthorResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiration);

        return Result.Success(response);
    }

	public async Task<Result<AuthorResponse>> GetRefreshTokenAsync(
	 string token,
	 string refreshToken,
	 CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.GetUserIdFromExpiredToken(token);

		if (userId is null)
			return Result.Failure<AuthorResponse>(UserErrors.InvalidJwtToken);

		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return Result.Failure<AuthorResponse>(UserErrors.InvalidJwtToken);

		var userRefreshToken = user.RefreshTokens
			.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);

		if (userRefreshToken is null)
			return Result.Failure<AuthorResponse>(UserErrors.InvalidRefreshToken);

		userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, Permission) = await GetRolesAndPermissions(user, cancellationToken);

        user.RefreshTokens.RemoveAll(x =>
			!x.IsActive || x.ExpiresOn <= DateTime.UtcNow);

		var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, Permission);

		var (newRefreshToken, refreshTokenExpiration) =
			await CreateRefreshTokenAsync(user);

		var response = new AuthorResponse(
			user.Id,
			user.Email,
			user.FirstName,
			user.LastName,
			newToken,
			expiresIn,
			newRefreshToken,
			refreshTokenExpiration);

		return Result.Success(response);
	}

	public async Task<Result> RevokeRefreshTokenAsync(
	 string token,
	 string refreshToken,
	 CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateToken(token);

		if (userId is null)
			return Result.Failure(UserErrors.InvalidJwtToken);

		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return Result.Failure(UserErrors.InvalidJwtToken);

		var userRefreshToken = user.RefreshTokens
			.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);

		if (userRefreshToken is null)
			return Result.Failure(UserErrors.InvalidRefreshToken);

		userRefreshToken.RevokedOn = DateTime.UtcNow;

		user.RefreshTokens.RemoveAll(x =>
			!x.IsActive || x.ExpiresOn <= DateTime.UtcNow);

		await _userManager.UpdateAsync(user);

		return Result.Success();
	}

    public async Task<Result<AuthorResponse>> RegisterAsync(
      RegisterRequest request,
      CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.Users
            .AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure<AuthorResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

       var result = await _userManager.CreateAsync(user, request.Password);

if (!result.Succeeded)
{
    var error = result.Errors.First();

    return Result.Failure<AuthorResponse>(
        new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
}

var addRoleResult = await _userManager.AddToRoleAsync(user, DefaultRoles.Member);

if (!addRoleResult.Succeeded)
{
    var error = addRoleResult.Errors.First();

    return Result.Failure<AuthorResponse>(
        new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
}
        var (userRoles, Permission) = await GetRolesAndPermissions(user, cancellationToken);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, Permission);

var (refreshToken, refreshTokenExpiration) =
    await CreateRefreshTokenAsync(user);

        var response = new AuthorResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiration);

        return Result.Success(response);
    }
    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Success();

        // Invalidate any previous unused codes
        var oldCodes = await _context.PasswordResetCodes
            .Where(x => x.UserId == user.Id && !x.Used)
            .ToListAsync();

        foreach (var item in oldCodes)
            item.Used = true;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var code = GenerateVerificationCode(5);
        var codeHash = ComputeSha256Hash(code + user.SecurityStamp);

        var entity = new PasswordResetCode
        {
            UserId = user.Id,
            CodeHash = codeHash,
            IdentityToken = encodedToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            CreatedAt = DateTime.UtcNow,
            Used = false
        };

        _context.PasswordResetCodes.Add(entity);
        await _context.SaveChangesAsync();


        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            "ForgetPassword",
            new Dictionary<string, string>
            {
            { "{{Code}}", code }
            });

        await _emailSender.SendEmailAsync(
            user.Email!,
            "Sakeena: Change Password",
            emailBody);

        return Result.Success();
    }

    public async Task<Result> VerifyResetCodeAsync(string email, string code)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Email == email);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        var resetEntry = await GetValidResetCodeAsync(user.Id);

        if (resetEntry is null)
            return Result.Failure(UserErrors.CodeReset);

        var providedHash = ComputeSha256Hash(code + user.SecurityStamp);

        if (!string.Equals(providedHash, resetEntry.CodeHash, StringComparison.Ordinal))
        {
            resetEntry.Attempts++;

            if (resetEntry.Attempts >= 3)
                resetEntry.Used = true;

            await _context.SaveChangesAsync();

            return Result.Failure(
                UserErrors.CodeReset with
                {
                    Description = "Invalid reset code"
                });
        }

        _logger.LogInformation(
            "Password reset code verified for user {UserId}",
            user.Id);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string code, string newPassword)
	{
		var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

		if (user is null)
			return Result.Failure(UserErrors.InvalidCode);

        var resetEntry = await GetValidResetCodeAsync(user.Id);

        if (resetEntry is null)
			return Result.Failure(UserErrors.CodeReset);

		var providedHash = ComputeSha256Hash(code + user.SecurityStamp);

		if (!string.Equals(providedHash, resetEntry.CodeHash, StringComparison.Ordinal))
		{
			resetEntry.Attempts = (resetEntry.Attempts) + 1;

			if (resetEntry.Attempts >= 3)
				resetEntry.Used = true;

			await _context.SaveChangesAsync();

			return Result.Failure(UserErrors.CodeReset with { Description = "Invalid reset code" });
		}

		if (string.IsNullOrEmpty(resetEntry.IdentityToken))
			return Result.Failure(UserErrors.CodeReset with { Description = "Reset token missing" });


		string identityToken;
		try
		{
			var tokenBytes = WebEncoders.Base64UrlDecode(resetEntry.IdentityToken);
			identityToken = Encoding.UTF8.GetString(tokenBytes);
		}
		catch
		{
			return Result.Failure(UserErrors.CodeReset with { Description = "Malformed reset token" });
		}

		var resetResult = await _userManager.ResetPasswordAsync(user, identityToken, newPassword);

		if (!resetResult.Succeeded)
		{
			var error = resetResult.Errors.First();

			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}

		resetEntry.Used = true;

		var others = await _context.PasswordResetCodes
			.Where(x => x.UserId == user.Id && !x.Used)
			.ToListAsync();

		foreach (var o in others)
			o.Used = true;

		await _context.SaveChangesAsync();

		await _userManager.UpdateSecurityStampAsync(user);

		_logger.LogInformation("Password reset completed for user {UserId}", user.Id);
		return Result.Success();
	}

	private static string GenerateVerificationCode(int length = 5)
	{
		var code = new char[length];
		var random = RandomNumberGenerator.GetBytes(length);
		for (int i = 0; i < length; i++)
		{
			var next = AllowedNumber._allowedNumber[random[i] % AllowedNumber._allowedNumber.Length];

			if (i > 0 && next == code[i - 1])
			{
				next = AllowedNumber._allowedNumber[(random[i] + 1) % AllowedNumber._allowedNumber.Length];
			}

			code[i] = next;
		}

		return new string(code);
	}

	private static string ComputeSha256Hash(string input)
	{
		var bytes = Encoding.UTF8.GetBytes(input);
		var hashed = SHA256.HashData(bytes);
		return Convert.ToBase64String(hashed);
	}

	private static string GenerateRefreshToken()
	{
		var refreshToken = RandomNumberGenerator.GetBytes(64);

		return Convert.ToBase64String(refreshToken);
	}

    private async Task<(string Token, DateTime Expiration)> CreateRefreshTokenAsync(ApplicationUser user)
    {
        var refreshToken = GenerateRefreshToken();
        var expiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = expiration
        });

        await _userManager.UpdateAsync(user);

        return (refreshToken, expiration);
    }

    private async Task<PasswordResetCode?> GetValidResetCodeAsync(string userId)
    {
        return await _context.PasswordResetCodes
            .Where(x => x.UserId == userId &&
                        !x.Used &&
                        x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

       private async Task <(IEnumerable<string> Roles,IEnumerable<string> Permission)> GetRolesAndPermissions(ApplicationUser user,CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var Permission = await(from r in _context.Roles
                                   join p in _context.RoleClaims
                                   on r.Id equals p.RoleId
                                   where userRoles.Contains(r.Name!)
                                   select p.ClaimValue)
                                    .Distinct()
                                    .ToListAsync(cancellationToken);
            return (userRoles, Permission);
        }
}