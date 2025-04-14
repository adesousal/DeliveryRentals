using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public class InMemoryMotoRepository : IMotoRepository
	{
		private readonly List<Motorcycle> _motos = new();

		public Task<bool> ExistsByPlateAsync(string plate)
		{
			return Task.FromResult(_motos.Any(m => m.LicensePlate == plate));
		}

		public Task AddAsync(Motorcycle moto)
		{
			_motos.Add(moto);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Motorcycle>> GetAllAsync()
		{
			return Task.FromResult(_motos.AsEnumerable());
		}

		public Task<Motorcycle?> GetByIdAsync(string id)
		{
			return Task.FromResult(_motos.FirstOrDefault(m => m.Id == id));
		}

		public Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
		{
			var moto = _motos.FirstOrDefault(m =>
				m.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));
			return Task.FromResult(moto);
		}

		public Task UpdateAsync(Motorcycle moto)
		{
			return Task.CompletedTask;
		}

		public Task DeleteAsync(string motoId)
		{
			var moto = _motos.FirstOrDefault(m => m.Id == motoId);
			if (moto != null)
				_motos.Remove(moto);

			return Task.CompletedTask;
		}
	}
}
