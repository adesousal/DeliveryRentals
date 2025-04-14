using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface ICourierRepository
	{
		Task<bool> ExistsByCnpjAsync(string cnpj);
		Task<bool> ExistsByCnhAsync(string cnhNumber);
		Task AddAsync(Courier courier);
		Task<Courier?> GetByIdAsync(string id);
		Task UpdateAsync(Courier courier);
	}
}
