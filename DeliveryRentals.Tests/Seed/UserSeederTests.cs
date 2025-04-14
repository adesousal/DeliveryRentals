using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Seed;
using Moq;

namespace DeliveryRentals.Tests.Seed
{
	public class UserSeederTests
	{
		[Fact]
		public async Task SeedAsync_ShouldNotAddUser_WhenUserAlreadyExists()
		{
			// Arrange
			var existingUser = new User("admin", "hashedpass", "Admin");
			var userToSeed = new User("admin", "ignored", "Admin");

			var repoMock = new Mock<IUserRepository>();
			repoMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(existingUser);

			// Act
			await UserSeeder.SeedAsync(repoMock.Object, userToSeed);

			// Assert
			repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
		}

		[Fact]
		public async Task SeedAsync_ShouldHashPasswordAndAddUser_WhenUserDoesNotExist()
		{
			// Arrange
			var userToSeed = new User("admin", "ignored", "Admin");

			var repoMock = new Mock<IUserRepository>();
			repoMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync((User?)null);

			User? addedUser = null;
			repoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
					.Callback<User>(u => addedUser = u)
					.Returns(Task.CompletedTask);

			// Act
			await UserSeeder.SeedAsync(repoMock.Object, userToSeed);

			// Assert
			repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
			Assert.NotNull(addedUser);
			Assert.Equal("admin", addedUser!.Username);
			Assert.Equal("Admin", addedUser.Role);
			Assert.True(BCrypt.Net.BCrypt.Verify("password123", addedUser.PasswordHash));
		}
	}
}
