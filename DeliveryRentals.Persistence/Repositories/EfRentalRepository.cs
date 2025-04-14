using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfRentalRepository : IRentalRepository
	{
		private readonly AppDbContext _db;

		public EfRentalRepository(AppDbContext db) => _db = db;

		public async Task AddAsync(Rental rental)
		{
			_db.Rentals.Add(rental);
			await _db.SaveChangesAsync();
		}

		public async Task<Rental?> GetByIdAsync(string id)
		{
			return await _db.Rentals.FindAsync(id);
		}

		public async Task UpdateAsync(Rental rental)
		{
			_db.Rentals.Update(rental);
			await _db.SaveChangesAsync();
		}

		public async Task<bool> HasRentalsByMotoIdAsync(string motoId)
		{
			return await _db.Rentals.AnyAsync(l => l.MotoId == motoId && DateTime.UtcNow < l.EndDate);
		}

		public async Task<IEnumerable<Rental>> GetAllAsync()
		{
			return await _db.Rentals.AsNoTracking().ToListAsync();
		}
	}
}
