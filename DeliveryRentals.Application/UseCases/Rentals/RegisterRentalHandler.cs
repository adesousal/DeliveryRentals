using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Rentals
{
	public class RegisterRentalHandler
	{
		private readonly ICourierRepository _courierRepository;
		private readonly IMotoRepository _motoRepository;
		private readonly IRentalRepository _rentalRepository;

		private readonly Dictionary<int, decimal> _planValues = new()
	{
		{ 7, 30m }, { 15, 28m }, { 30, 22m }, { 45, 20m }, { 50, 18m }
	};

		public RegisterRentalHandler(
			ICourierRepository courierRepository,
			IMotoRepository motoRepository,
			IRentalRepository rentalRepository)
		{
			_courierRepository = courierRepository;
			_motoRepository = motoRepository;
			_rentalRepository = rentalRepository;
		}

		public async Task<RegisterRentalResponse> HandleAsync(RegisterRentalRequest request)
		{
			var moto = await _motoRepository.GetByIdAsync(request.MotoId)
				?? throw new InvalidOperationException("Moto not found");

			var courier = await _courierRepository.GetByIdAsync(request.CourierId)
				?? throw new InvalidOperationException("Courier not found");

			if (!courier.CnhType.Contains("A"))
				throw new InvalidOperationException("Courier must have CNH type A");

			if (!_planValues.ContainsKey(request.Plan))
				throw new InvalidOperationException("Invalid plan");

			if (await _rentalRepository.HasRentalsByMotoIdAsync(moto.Id))
				throw new InvalidOperationException("This motorcycle is already rented");

			var StartDate = DateTime.UtcNow.Date.AddDays(1);

			var dailyValue = _planValues[request.Plan];

			var rental = new Rental(
				id: Guid.NewGuid().ToString(),
				motoId: request.MotoId,
				courierId: request.CourierId,
				start: StartDate,
				end: StartDate.AddDays(request.Plan),
				forecastTerminus: StartDate.AddDays(request.Plan),
				plan: request.Plan,
				dailyValue: dailyValue
			);

			await _rentalRepository.AddAsync(rental);

			return new RegisterRentalResponse()
			{
				MotoId = rental.MotoId,
				CourierId = rental.CourierId,
				StartDate = rental.StartDate,				
				ForecastDateEnd = rental.ForecastDateEnd,
				Plan = $"{rental.Plan} days",
				DailyValue = $"$ {rental.DailyValue}",
				TotalExpectedValue = $"$ {rental.DailyValue * rental.Plan}"
			};
		}
	}
}
