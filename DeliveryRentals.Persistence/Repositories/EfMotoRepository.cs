using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfMotoRepository : IMotoRepository
	{
		private readonly AppDbContext _db;

		public EfMotoRepository(AppDbContext db) => _db = db;

		public async Task AddAsync(Motorcycle moto)
		{
			_db.Motorcycles.Add(moto);
			await _db.SaveChangesAsync();
		}

		public async Task<IEnumerable<Motorcycle>> GetAllAsync()
		{
			return await _db.Motorcycles.AsNoTracking().ToListAsync();
		}

		public async Task<Motorcycle?> GetByIdAsync(string id)
		{
			return await _db.Motorcycles.FindAsync(id);
		}

		public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
		{
			return await _db.Motorcycles
				.FirstOrDefaultAsync(m => m.LicensePlate.ToLower() == licensePlate.ToLower());
		}

		public async Task<bool> ExistsByPlateAsync(string licensePlate)
		{
			return await _db.Motorcycles
				.AnyAsync(m => m.LicensePlate.ToLower() == licensePlate.ToLower());
		}

		public async Task UpdateAsync(Motorcycle moto)
		{
			_db.Motorcycles.Update(moto);
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			var moto = await _db.Motorcycles.FindAsync(id);
			if (moto != null)
			{
				_db.Motorcycles.Remove(moto);
				await _db.SaveChangesAsync();
			}
		}
	}
}
