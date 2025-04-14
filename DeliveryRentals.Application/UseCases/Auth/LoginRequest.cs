
namespace DeliveryRentals.Application.UseCases.Auth
{
	public class LoginRequest
	{
		public string Username { get; set; } = string.Empty; // CNPJ ou admin
		public string Password { get; set; } = string.Empty; // apenas simulado
	}
}
