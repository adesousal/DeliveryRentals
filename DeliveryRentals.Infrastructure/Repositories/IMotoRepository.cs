using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface IMotoRepository
	{
		Task AddAsync(Motorcycle moto);
		Task<IEnumerable<Motorcycle>> GetAllAsync();
		Task<Motorcycle?> GetByIdAsync(string id);
		Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate);
		Task<bool> ExistsByPlateAsync(string licensePlate);
		Task UpdateAsync(Motorcycle moto);
		Task DeleteAsync(string id);
	}
}
