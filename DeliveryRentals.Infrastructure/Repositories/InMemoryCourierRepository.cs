using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public class InMemoryCourierRepository : ICourierRepository
	{
		private readonly List<Courier> _couriers = new();

		public Task<bool> ExistsByCnpjAsync(string cnpj)
		{
			return Task.FromResult(_couriers.Any(e => e.Cnpj == cnpj));
		}

		public Task<bool> ExistsByCnhAsync(string cnhNumber)
		{
			return Task.FromResult(_couriers.Any(e => e.CnhNumber == cnhNumber));
		}

		public Task AddAsync(Courier courier)
		{
			_couriers.Add(courier);
			return Task.CompletedTask;
		}

		public Task<Courier?> GetByIdAsync(string id)
		{
			return Task.FromResult(_couriers.FirstOrDefault(e => e.Id == id));
		}

		public Task UpdateAsync(Courier courier)
		{
			// No in-memory implementation, o objeto já é referenciado.
			return Task.CompletedTask;
		}
	}
}
