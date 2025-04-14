using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DeliveryRentals.Infrastructure.Security
{
	public class RoleHandler : AuthorizationHandler<RoleRequirement>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public RoleHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			RoleRequirement requirement)
		{
			var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

			if (userRole == requirement.Role)
			{
				context.Succeed(requirement);
			}
			else
			{
				var httpContext = _httpContextAccessor.HttpContext;
				if (httpContext != null)
				{
					httpContext.Items["AccessDeniedMessage"] = requirement.Message;
				}
				context.Fail();
			}

			return Task.CompletedTask;
		}
	}
}
