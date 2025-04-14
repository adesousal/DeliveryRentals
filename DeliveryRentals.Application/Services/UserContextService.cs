using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DeliveryRentals.Application.Services
{
	public class UserContextService : IUserContextService
	{
		private readonly IHttpContextAccessor _accessor;

		public UserContextService(IHttpContextAccessor accessor)
		{
			_accessor = accessor;
		}

		public string? UserId => _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		public string? Name => _accessor.HttpContext?.User.Identity?.Name;
		public string? Role => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
		public bool HasUser => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Name);
	}
}
