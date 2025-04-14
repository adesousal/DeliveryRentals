using DeliveryRentals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Context
{
	public class AppDbContext : DbContext
	{
		public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
		public DbSet<Courier> Couriers => Set<Courier>();
		public DbSet<Rental> Rentals => Set<Rental>();
		public DbSet<User> Users => Set<User>();
		public DbSet<LogEntry> Logs => Set<LogEntry>();
		public DbSet<Events> Events => Set<Events>();

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Motorcycle>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.LicensePlate).IsUnique();
			});

			modelBuilder.Entity<Courier>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Cnpj).IsUnique();
				entity.HasIndex(e => e.CnhNumber).IsUnique();
				entity.Property(e => e.BirthDate)
					.HasColumnType("timestamp without time zone");
			});

			modelBuilder.Entity<Rental>().HasKey(e => e.Id);

			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => e.Username).IsUnique();
			});

			modelBuilder.Entity<LogEntry>().HasKey(x => x.Id);

			modelBuilder.Entity<Events>().HasKey(x => x.Id);
		}
	}
}
