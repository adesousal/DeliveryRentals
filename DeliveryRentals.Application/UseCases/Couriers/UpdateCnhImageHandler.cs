
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Storage;

namespace DeliveryRentals.Application.UseCases.Couriers
{
	public class UpdateCnhImageHandler
	{
		private readonly ICourierRepository _repository;
		private readonly ICnhStorageService _storage;

		public UpdateCnhImageHandler(ICourierRepository repository, ICnhStorageService storage)
		{
			_repository = repository;
			_storage = storage;
		}

		public async Task HandleAsync(string courierId, Stream imageStream, string fileName)
		{
			var courier = await _repository.GetByIdAsync(courierId);
			if (courier == null)
				throw new InvalidOperationException("Courier not found.");

			if (!fileName.EndsWith(".png") && !fileName.EndsWith(".bmp"))
				throw new InvalidOperationException("Invalid file format.");

			var savedPath = await _storage.SaveAsync(courierId, imageStream, fileName);
			courier.UpdateCnhImage(savedPath);

			await _repository.UpdateAsync(courier);
		}
	}
}
