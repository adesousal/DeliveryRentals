using DeliveryRentals.Application.Services;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using Moq;

namespace DeliveryRentals.Tests.Services
{
	public class LoggingServiceTests
	{
		private readonly Mock<ILogRepository> _repoMock;
		private readonly Mock<IUserContextService> _contextMock;
		private readonly LoggingService _service;

		public LoggingServiceTests()
		{
			_repoMock = new Mock<ILogRepository>();
			_contextMock = new Mock<IUserContextService>();
			_service = new LoggingService(_repoMock.Object, _contextMock.Object);
		}

		[Fact]
		public async Task Info_WithExplicitUser_ShouldLogWithUser()
		{
			// Arrange
			var message = "Test info message";
			var user = "explicitUser";

			// Act
			await _service.Info(message, user);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Info" &&
				l.Message == message &&
				l.User == user &&
				l.Role == null
			)), Times.Once);
		}

		[Fact]
		public async Task Info_WithUserContext_ShouldLogWithContextUser()
		{
			// Arrange
			var message = "Context user info";
			_contextMock.Setup(c => c.HasUser).Returns(true);
			_contextMock.Setup(c => c.UserId).Returns("123");
			_contextMock.Setup(c => c.Name).Returns("John");

			// Act
			await _service.Info(message);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Info" &&
				l.Message == message &&
				l.User == "123 - John"
			)), Times.Once);
		}

		[Fact]
		public async Task Info_WithoutUserOrContext_ShouldLogWithoutUser()
		{
			// Arrange
			var message = "No user info";
			_contextMock.Setup(c => c.HasUser).Returns(false);

			// Act
			await _service.Info(message);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Info" &&
				l.Message == message &&
				l.User == null
			)), Times.Once);
		}

		[Fact]
		public async Task Error_WithExceptionAndExplicitUser_ShouldLogWithDetails()
		{
			// Arrange
			var message = "Error occurred";
			var ex = new InvalidOperationException("Something went wrong");
			var user = "errorUser";

			// Act
			await _service.Error(message, ex, user);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Error" &&
				l.Message == message &&
				l.Exception == ex.ToString() &&
				l.User == user
			)), Times.Once);
		}

		[Fact]
		public async Task Error_WithUserContext_ShouldLogWithContextUser()
		{
			// Arrange
			var message = "Context error";
			var ex = new Exception("Contextual error");
			_contextMock.Setup(c => c.HasUser).Returns(true);
			_contextMock.Setup(c => c.UserId).Returns("456");
			_contextMock.Setup(c => c.Name).Returns("Doe");

			// Act
			await _service.Error(message, ex);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Error" &&
				l.Message == message &&
				l.Exception == ex.ToString() &&
				l.User == "456 - Doe"
			)), Times.Once);
		}

		[Fact]
		public async Task Error_WithoutUserOrContext_ShouldLogWithoutUser()
		{
			// Arrange
			var message = "Error with no user";
			var ex = new Exception("No user exception");
			_contextMock.Setup(c => c.HasUser).Returns(false);

			// Act
			await _service.Error(message, ex);

			// Assert
			_repoMock.Verify(r => r.AddAsync(It.Is<LogEntry>(l =>
				l.Level == "Error" &&
				l.Message == message &&
				l.Exception == ex.ToString() &&
				l.User == null &&
				l.Role == null
			)), Times.Once);
		}
	}
}
