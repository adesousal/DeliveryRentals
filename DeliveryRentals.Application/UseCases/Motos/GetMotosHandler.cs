using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Motos
{
	public class GetMotosHandler
	{
		private readonly IMotoRepository _repository;

		public GetMotosHandler(IMotoRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<MotoDto>> HandleAsync(GetMotosQuery query)
		{
			var motos = await _repository.GetAllAsync();

			if (!string.IsNullOrWhiteSpace(query.LicensePlate))
				motos = motos.Where(m => m.LicensePlate.Contains(query.LicensePlate, StringComparison.OrdinalIgnoreCase));

			return motos.Select(m => new MotoDto
			{
				Id = m.Id,
				Year = m.Year,
				Model = m.Model,
				LicensePlate = m.LicensePlate
			});
		}
	}
}
