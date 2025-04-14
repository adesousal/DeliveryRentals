using DeliveryRentals.Application.UseCases.Auth;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DeliveryRentals.Tests.UseCases.Auth
{
	public class LoginHandlerTests
	{
		private static LoginHandler CreateHandler(User? user = null, bool existsCourier = true)
		{
			var userRepoMock = new Mock<IUserRepository>();
			userRepoMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
				.ReturnsAsync(user);

			var courierRepoMock = new Mock<ICourierRepository>();
			courierRepoMock.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>()))
				.ReturnsAsync(existsCourier);

			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string>
				{
					{ "Jwt:Key", "V3ryS3cur3JWTKeyThatIsLongEnough2024!" }
				}).Build();

			return new LoginHandler(userRepoMock.Object, courierRepoMock.Object, config);
		}

		[Fact]
		public async Task Must_authenticate_admin_successfully()
		{
			var hash = BCrypt.Net.BCrypt.HashPassword("admin123");
			var user = new User("admin", hash, "admin");

			var handler = CreateHandler(user);

			var result = await handler.HandleAsync(new LoginRequest
			{
				Username = "admin",
				Password = "admin123"
			});

			result.Role.Should().Be("admin");
			result.Token.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Should_fail_when_admin_has_incorrect_password()
		{
			var user = new User("admin", BCrypt.Net.BCrypt.HashPassword("correct"), "admin");

			var handler = CreateHandler(user);

			var act = async () => await handler.HandleAsync(new LoginRequest
			{
				Username = "admin",
				Password = "wrong"
			});

			await act.Should().ThrowAsync<UnauthorizedAccessException>()
				.WithMessage("Invalid credentials.");
		}

		[Fact]
		public async Task Must_authenticate_courier_when_cnpj_exists_and_no_user()
		{
			var handler = CreateHandler(user: null, existsCourier: true);

			var result = await handler.HandleAsync(new LoginRequest
			{
				Username = "12345678000191",
				Password = "any"
			});

			result.Role.Should().Be("courier");
			result.Token.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Must_fail_if_courier_does_not_exist()
		{
			var handler = CreateHandler(user: null, existsCourier: false);

			var act = async () => await handler.HandleAsync(new LoginRequest
			{
				Username = "99999999000191",
				Password = "senha"
			});

			await act.Should().ThrowAsync<UnauthorizedAccessException>()
				.WithMessage("Courier not found.");
		}

		[Fact]
		public async Task Must_fail_if_user_does_not_exist()
		{
			var handler = CreateHandler(user: null, existsCourier: false);

			var act = async () => await handler.HandleAsync(new LoginRequest
			{
				Username = "admin",
				Password = "any"
			});

			await act.Should().ThrowAsync<UnauthorizedAccessException>();
		}
	}
}
