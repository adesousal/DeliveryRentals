
using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface IRentalRepository
	{
		Task<bool> HasRentalsByMotoIdAsync(string motoId);
		Task AddAsync(Rental rental);
		Task<Rental?> GetByIdAsync(string id);
		Task UpdateAsync(Rental rental);
	}
}
