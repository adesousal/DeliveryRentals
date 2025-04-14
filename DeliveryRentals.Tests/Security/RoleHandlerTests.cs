using DeliveryRentals.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace DeliveryRentals.Tests.Security
{
	public class RoleHandlerTests
	{
		[Fact]
		public async Task Must_authorize_when_role_is_validated()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			var httpContextAccessor = new Mock<IHttpContextAccessor>();
			httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

			var requirement = new RoleRequirement("admin", "Message");
			var handler = new RoleHandler(httpContextAccessor.Object);

			var claims = new[] { new Claim(ClaimTypes.Role, "admin") };
			var identity = new ClaimsIdentity(claims, "test");
			var user = new ClaimsPrincipal(identity);

			var authContext = new AuthorizationHandlerContext(
				new[] { requirement }, user, null);

			// Act
			await handler.HandleAsync(authContext);

			// Assert
			authContext.HasSucceeded.Should().BeTrue();
		}

		[Fact]
		public async Task Must_fail_and_save_message_when_role_does_not_match()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			var httpContextAccessor = new Mock<IHttpContextAccessor>();
			httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

			var requirement = new RoleRequirement("courier", "Only couriers can rent.");
			var handler = new RoleHandler(httpContextAccessor.Object);

			var claims = new[] { new Claim(ClaimTypes.Role, "admin") };
			var identity = new ClaimsIdentity(claims, "test");
			var user = new ClaimsPrincipal(identity);

			var authContext = new AuthorizationHandlerContext(
				new[] { requirement }, user, null);

			// Act
			await handler.HandleAsync(authContext);

			// Assert
			authContext.HasSucceeded.Should().BeFalse();
			httpContext.Items["AccessDeniedMessage"].Should().Be("Only couriers can rent.");
		}
	}
}
