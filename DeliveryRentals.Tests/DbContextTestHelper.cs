using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Tests
{
	public static class DbContextTestHelper
	{
		public static AppDbContext CreateInMemoryContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite("Filename=:memory:")
				.Options;

			var context = new AppDbContext(options);
			context.Database.OpenConnection();
			context.Database.EnsureCreated();

			return context;
		}
	}
}
