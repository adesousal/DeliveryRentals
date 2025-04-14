using DeliveryRentals.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryRentals.API.Controllers
{
	[Route("auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly LoginHandler _handler;

		public AuthController(LoginHandler handler)
		{
			_handler = handler;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var response = await _handler.HandleAsync(request);
			return Ok(response);
		}
	}
}
