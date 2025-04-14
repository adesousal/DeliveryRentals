using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfCourierRepository : ICourierRepository
	{
		private readonly AppDbContext _db;

		public EfCourierRepository(AppDbContext db) => _db = db;

		public async Task AddAsync(Courier courier)
		{
			_db.Couriers.Add(courier);
			await _db.SaveChangesAsync();
		}

		public async Task<Courier?> GetByIdAsync(string id)
		{
			return await _db.Couriers.FindAsync(id);
		}

		public async Task<bool> ExistsByCnpjAsync(string cnpj)
		{
			return await _db.Couriers.AnyAsync(e => e.Cnpj == cnpj);
		}

		public async Task<bool> ExistsByCnhAsync(string cnhNumber)
		{
			return await _db.Couriers.AnyAsync(e => e.CnhNumber == cnhNumber);
		}

		public async Task UpdateAsync(Courier courier)
		{
			_db.Couriers.Update(courier);
			await _db.SaveChangesAsync();
		}
	}
}
