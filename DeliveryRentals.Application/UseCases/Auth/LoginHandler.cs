using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace DeliveryRentals.Application.UseCases.Auth
{
	public class LoginHandler
	{
		private readonly IUserRepository _userRepository;
		private readonly ICourierRepository _courierRepository;
		private readonly IConfiguration _config;

		public LoginHandler(
			IUserRepository userRepository,
			ICourierRepository courierRepository,
			IConfiguration config)
		{
			_userRepository = userRepository;
			_courierRepository = courierRepository;
			_config = config;
		}

		public async Task<LoginResponse> HandleAsync(LoginRequest request)
		{
			var jwtKey = _config["Jwt:Key"] ?? "default-key";

			var user = await _userRepository.GetByUsernameAsync(request.Username);
			if (user != null)
			{
				if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
					throw new UnauthorizedAccessException("Invalid credentials.");

				return new LoginResponse
				{
					Token = TokenService.GenerateToken(user.Username, user.Role, jwtKey),
					Role = user.Role
				};
			}

			// Fallback to courier (login by CNPJ)
			var existCourier = await _courierRepository.ExistsByCnpjAsync(request.Username);
			if (!existCourier)
				throw new UnauthorizedAccessException("Courier not found.");

			return new LoginResponse
			{
				Token = TokenService.GenerateToken(request.Username, "courier", jwtKey),
				Role = "courier"
			};
		}
	}
}
