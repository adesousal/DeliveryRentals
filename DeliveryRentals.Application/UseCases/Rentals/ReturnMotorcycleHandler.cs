using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Rentals
{
	public class ReturnMotorcycleHandler
	{
		private readonly IRentalRepository _rentalRepository;

		public ReturnMotorcycleHandler(IRentalRepository rentalRepository)
		{
			_rentalRepository = rentalRepository;
		}

		public async Task<ReturnMotoResponse> HandleAsync(ReturnMotorcycleRequest request)
		{
			var rental = await _rentalRepository.GetByIdAsync(request.RentalId)
				?? throw new InvalidOperationException("Rental not found");

			rental.RegistrarDevolucao(request.ReturnDate);
			await _rentalRepository.UpdateAsync(rental);

			var usedDays = (request.ReturnDate.Date - rental.StartDate.Date).Days + 1;
			var baseValue = usedDays * rental.DailyValue;

			if (request.ReturnDate.Date < rental.ForecastDateEnd.Date)
			{
				var daysRemaining = (rental.ForecastDateEnd.Date - request.ReturnDate.Date).Days;
				decimal fine = 0;

				if (rental.Plan == 7)
					fine = daysRemaining * rental.DailyValue * 0.20m;
				else if (rental.Plan == 15)
					fine = daysRemaining * rental.DailyValue * 0.40m;

				return new ReturnMotoResponse
				{
					TotalValue = baseValue + fine,
					Description = $"Early return. Fine applied: R${fine:N2}"
				};
			}
			else if (request.ReturnDate.Date > rental.ForecastDateEnd.Date)
			{
				var additionalDays = (request.ReturnDate.Date - rental.ForecastDateEnd.Date).Days;
				var additional = additionalDays * 50;

				return new ReturnMotoResponse
				{
					TotalValue = baseValue + additional,
					Description = $"Late return. Additional charge: R${additional:N2}"
				};
			}

			return new ReturnMotoResponse
			{
				TotalValue = baseValue,
				Description = "Return within the deadline."
			};
		}
	}
}
