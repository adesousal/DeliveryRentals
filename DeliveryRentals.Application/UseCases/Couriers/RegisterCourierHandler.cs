using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Storage;

namespace DeliveryRentals.Application.UseCases.Couriers
{
	public class RegisterCourierHandler
	{
		private readonly ICourierRepository _repository;
		private readonly ICnhStorageService _storage;

		public RegisterCourierHandler(ICourierRepository repository, ICnhStorageService storage)
		{
			_repository = repository;
			_storage = storage;
		}

		public async Task HandleAsync(RegisterCourierRequest request)
		{
			if (!DateTime.TryParse(request.BirthDate.ToString(), out var parsedDate))
				throw new InvalidOperationException("Invalid date of birth format");

			if (request.BirthDate > DateTime.UtcNow)
				throw new InvalidOperationException("Invalid date of birth");

			if (request.BirthDate > DateTime.UtcNow.AddYears(-18))
				throw new InvalidOperationException("Courier must be at least 18 years old");

			if (await _repository.ExistsByCnpjAsync(request.Cnpj))
				throw new InvalidOperationException("CNPJ already registered");

			if (await _repository.ExistsByCnhAsync(request.CnhNumber))
				throw new InvalidOperationException("CNH number already registered");

			if (!new[] { "A", "B", "A+B" }.Contains(request.CnhType))
				throw new InvalidOperationException("Invalid CNH type");

			var courier = new Courier(
				request.Id,
				request.Name,
				request.Cnpj,
				request.BirthDate,
				request.CnhNumber,
				request.CnhType
			);

			await _repository.AddAsync(courier);

			if (request.CnhImage != null)
			{
				var imagePath = await _storage.SaveAsync(
					request.Id,
					request.CnhImage.OpenReadStream(),
					request.CnhImage.FileName);

				courier.UpdateCnhImage(imagePath);
			}
		}
	}
}
