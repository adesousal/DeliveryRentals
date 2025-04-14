using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface ILogRepository
	{
		Task AddAsync(LogEntry log);
		Task<IEnumerable<LogEntry>> SearchAsync(string? user = null,
												DateTime? from = null,
												DateTime? to = null,
												int take = 100,
												int skip = 0);
	}
}
