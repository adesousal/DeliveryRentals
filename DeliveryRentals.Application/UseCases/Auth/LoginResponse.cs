﻿
namespace DeliveryRentals.Application.UseCases.Auth
{
	public class LoginResponse
	{
		public string Token { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
	}
}
