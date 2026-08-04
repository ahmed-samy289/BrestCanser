namespace BrestCanser.Api.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{

	public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
	public DbSet<PredictionHistory> PredictionHistories { get; set; }
	public DbSet<Notification> Notifications { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		var cascadeFKs = modelBuilder.Model.GetEntityTypes()
			.SelectMany(t => t.GetForeignKeys())
			.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

		foreach (var fk in cascadeFKs)
			fk.DeleteBehavior = DeleteBehavior.Restrict;

		base.OnModelCreating(modelBuilder);
		// Configure entity properties and relationships here if needed
	}

}
