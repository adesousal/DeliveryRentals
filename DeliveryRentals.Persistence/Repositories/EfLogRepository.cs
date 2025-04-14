using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfLogRepository : ILogRepository
	{
		private readonly AppDbContext _context;

		public EfLogRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(LogEntry log)
		{
			await _context.Logs.AddAsync(log);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<LogEntry>> SearchAsync(string? user = null,
															 DateTime? from = null,
															 DateTime? to = null,
															 int take = 100,
															 int skip = 0)
		{
			var query = _context.Logs.AsQueryable();

			if (!string.IsNullOrWhiteSpace(user))
				query = query.Where(l => l.User!.ToLower().Contains(user.ToLower()));

			if (from.HasValue)
				query = query.Where(l => l.Timestamp >= from.Value);

			if (to.HasValue)
				query = query.Where(l => l.Timestamp <= to.Value);

			return await query
				.OrderByDescending(l => l.Timestamp)
				.Skip(skip)
				.Take(take)
				.ToListAsync();
		}
	}
}
