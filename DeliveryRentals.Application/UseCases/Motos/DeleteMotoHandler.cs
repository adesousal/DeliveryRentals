using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Motos
{
	public class DeleteMotoHandler
	{
		private readonly IMotoRepository _motoRepository;
		private readonly IRentalRepository _rentalRepository;

		public DeleteMotoHandler(IMotoRepository motoRepository, IRentalRepository rentalRepository)
		{
			_motoRepository = motoRepository;
			_rentalRepository = rentalRepository;
		}

		public async Task HandleAsync(string motoId)
		{
			var moto = await _motoRepository.GetByIdAsync(motoId);
			if (moto == null)
				throw new InvalidOperationException("Moto not found");

			var hasLocacoes = await _rentalRepository.HasRentalsByMotoIdAsync(motoId);
			if (hasLocacoes)
				throw new InvalidOperationException("Cannot delete moto with existing rentals");

			await _motoRepository.DeleteAsync(motoId);
		}
	}
}
