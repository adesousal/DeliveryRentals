using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.Services
{
	public class LoggingService
	{
		private readonly ILogRepository _repo;
		private readonly IUserContextService _context;

		public LoggingService(ILogRepository repo, IUserContextService context)
		{
			_repo = repo;
			_context = context;
		}

		public Task Info(string message, string? user = null) =>
			_repo.AddAsync(new LogEntry
			{
				Level = "Info",
				Message = message,
				User = user ?? (_context.HasUser ? $"{_context.UserId} - {_context.Name}" : null)
			});

		public Task Error(string message, Exception ex, string? user = null) =>
			_repo.AddAsync(new LogEntry
			{
				Level = "Error",
				Message = message,
				Exception = ex.ToString(),
				User = user ?? (_context.HasUser ? $"{_context.UserId} - {_context.Name}" : null)
			});
	}
}
