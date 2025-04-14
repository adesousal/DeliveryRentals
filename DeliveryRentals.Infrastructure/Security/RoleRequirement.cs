
using Microsoft.AspNetCore.Authorization;

namespace DeliveryRentals.Infrastructure.Security
{
	public class RoleRequirement : IAuthorizationRequirement
	{
		public string Role { get; }
		public string Message { get; }

		public RoleRequirement(string role, string message)
		{
			Role = role;
			Message = message;
		}
	}
}
