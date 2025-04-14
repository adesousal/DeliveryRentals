using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Motos
{
	public class UpdateMotoPlateHandler
	{
		private readonly IMotoRepository _repository;

		public UpdateMotoPlateHandler(IMotoRepository repository)
		{
			_repository = repository;
		}

		public async Task HandleAsync(UpdateMotoPlateRequest request)
		{
			var moto = await _repository.GetByIdAsync(request.MotoId);
			if (moto == null)
				throw new InvalidOperationException("Moto not found");

			if (await _repository.ExistsByPlateAsync(request.NewLicensePlate))
				throw new InvalidOperationException("License plate already in use");

			moto.UpdatePlate(request.NewLicensePlate);
			await _repository.UpdateAsync(moto);
		}
	}
}
