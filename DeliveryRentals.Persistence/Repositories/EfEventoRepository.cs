using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfEventoRepository : IEventRepository
	{
		private readonly AppDbContext _context;

		public EfEventoRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveEventAsync(string json)
		{

			await _context.Events.AddAsync(new Events(json));
			await _context.SaveChangesAsync();
		}
		
		public async Task<IEnumerable<string>> GetEvents(DateTime? from = null,
														 DateTime? to = null,
														 int take = 100,
														 int skip = 0) {

			var query = _context.Events.AsQueryable();

			if (from.HasValue)
				query = query.Where(l => l.Timestamp >= from.Value);

			if (to.HasValue)
				query = query.Where(l => l.Timestamp <= to.Value);

			return await query
				.OrderByDescending(l => l.Timestamp)
				.Select(l => l.Dados)
				.Skip(skip)
				.Take(take)
				.ToListAsync();
		}
	}
}
