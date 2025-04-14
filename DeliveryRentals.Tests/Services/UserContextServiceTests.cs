using DeliveryRentals.Application.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace DeliveryRentals.Tests.Services
{
	public class UserContextServiceTests
	{
		private readonly Mock<IHttpContextAccessor> _accessorMock;
		private readonly UserContextService _service;

		public UserContextServiceTests()
		{
			_accessorMock = new Mock<IHttpContextAccessor>();
			_service = new UserContextService(_accessorMock.Object);
		}

		private static ClaimsPrincipal CreateClaimsPrincipal(string? userId, string? name, string? role = null)
		{
			var claims = new List<Claim>();
			if (userId != null)
				claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
			if (name != null)
				claims.Add(new Claim(ClaimTypes.Name, name));
			if (role != null)
				claims.Add(new Claim(ClaimTypes.Role, role));

			var identity = new ClaimsIdentity(claims, "mock");
			return new ClaimsPrincipal(identity);
		}

		[Fact]
		public void UserId_ShouldReturnCorrectValue()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal("123", "Alice")
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var userId = _service.UserId;

			// Assert
			Assert.Equal("123", userId);
		}

		[Fact]
		public void Name_ShouldReturnCorrectValue()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal("123", "Alice")
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var name = _service.Name;

			// Assert
			Assert.Equal("Alice", name);
		}

		[Fact]
		public void Role_ShouldReturnCorrectValue()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal("123", "Alice", "Admin")
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var role = _service.Role;

			// Assert
			Assert.Equal("Admin", role);
		}

		[Fact]
		public void HasUser_ShouldReturnTrue_WhenUserIdAndNameArePresent()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal("123", "Alice")
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var hasUser = _service.HasUser;

			// Assert
			Assert.True(hasUser);
		}

		[Fact]
		public void HasUser_ShouldReturnFalse_WhenUserIdIsMissing()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal(null, "Alice")
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var hasUser = _service.HasUser;

			// Assert
			Assert.False(hasUser);
		}

		[Fact]
		public void HasUser_ShouldReturnFalse_WhenNameIsMissing()
		{
			// Arrange
			var httpContext = new DefaultHttpContext
			{
				User = CreateClaimsPrincipal("123", null)
			};

			_accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

			// Act
			var hasUser = _service.HasUser;

			// Assert
			Assert.False(hasUser);
		}

		[Fact]
		public void AllProperties_ShouldReturnNull_WhenHttpContextIsNull()
		{
			// Arrange
			_accessorMock.Setup(a => a.HttpContext).Returns<HttpContext>(null);

			// Act & Assert
			Assert.Null(_service.UserId);
			Assert.Null(_service.Name);
			Assert.Null(_service.Role);
			Assert.False(_service.HasUser);
		}
	}
}
