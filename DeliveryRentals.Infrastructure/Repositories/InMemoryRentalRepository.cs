
using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public class InMemoryRentalRepository : IRentalRepository
	{
		private readonly List<Rental> _rentals = new();

		public Task AddAsync(Rental rental)
		{
			_rentals.Add(rental);
			return Task.CompletedTask;
		}

		public Task<bool> HasRentalsByMotoIdAsync(string motoId)
		{
			var exists = _rentals.Any(x => x.MotoId == motoId);
			return Task.FromResult(exists);
		}
		
		public void AddRental(string motoId, string courierId)
		{
			_rentals.Add(new Rental(
				id: Guid.NewGuid().ToString(),
				motoId: motoId,
				courierId: courierId,
				start: DateTime.UtcNow,
				end: DateTime.UtcNow.AddDays(1),
				forecastTerminus: DateTime.UtcNow.AddDays(1),
				plan: 7,
				dailyValue: 30m
			));
		}

		public Task<Rental?> GetByIdAsync(string id)
		{
			return Task.FromResult(_rentals.FirstOrDefault(l => l.Id == id));
		}

		public Task UpdateAsync(Rental rental)
		{
			// In-memory — nothing to do
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Rental>> GetAllAsync()
		{
			return Task.FromResult(_rentals.AsEnumerable());
		}
	}
}
