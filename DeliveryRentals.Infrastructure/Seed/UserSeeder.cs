using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Infrastructure.Seed
{
	public static class UserSeeder
	{
		public static async Task SeedAsync(IUserRepository repository, User user)
		{
			var existing = await repository.GetByUsernameAsync(user.Username);
			if (existing is not null) return;

			var hash = BCrypt.Net.BCrypt.HashPassword("password123");
			user = new User(user.Username, hash, user.Role);

			await repository.AddAsync(user);
		}
	}
}
