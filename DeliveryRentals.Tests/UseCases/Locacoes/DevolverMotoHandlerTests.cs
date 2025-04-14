using DeliveryRentals.Application.UseCases.Rentals;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Locacoes
{
	public class DevolverMotoHandlerTests
	{
		[Fact]
		public async Task Must_apply_normal_rate_if_returned_on_time()
		{
			var repo = new InMemoryRentalRepository();
			var start = DateTime.UtcNow.Date;
			var end = start.AddDays(6);

			var rental = new Rental(
				id: "loc1",
				motoId: "moto1",
				courierId: "ent1",
				start: start,
				end: end,
				forecastTerminus: end,
				plan: 7,
				dailyValue: 30m
			);

			await repo.AddAsync(rental);

			var handler = new ReturnMotorcycleHandler(repo);
			var result = await handler.HandleAsync(new ReturnMotorcycleRequest
			{
				RentalId = "loc1",
				ReturnDate = end
			});

			result.TotalValue.Should().Be(7 * 30m);
			result.Description.Should().Contain("deadline");
		}

		[Fact]
		public async Task Must_impose_fee_for_early_return_in_7_day_scheme()
		{
			var repo = new InMemoryRentalRepository();
			var start = DateTime.UtcNow.Date;
			var forecast = start.AddDays(6);

			var rental = new Rental(
				id: "loc2",
				motoId: "moto2",
				courierId: "ent2",
				start: start,
				end: forecast,
				forecastTerminus: forecast,
				plan: 7,
				dailyValue: 30m
			);

			await repo.AddAsync(rental);

			var handler = new ReturnMotorcycleHandler(repo);
			var result = await handler.HandleAsync(new ReturnMotorcycleRequest
			{
				RentalId = "loc2",
				ReturnDate = start.AddDays(3)
			});

			var daysRemaining = 3;
			var fine = daysRemaining * 30m * 0.20m;

			result.TotalValue.Should().Be((4 * 30m) + fine);
			result.Description.Should().Contain("Fine");
		}

		[Fact]
		public async Task Must_impose_surcharge_if_returned_late()
		{
			var repo = new InMemoryRentalRepository();
			var start = DateTime.UtcNow.Date;
			var forecast = start.AddDays(14);

			var rental = new Rental(
				id: "loc3",
				motoId: "moto3",
				courierId: "ent3",
				start: start,
				end: forecast,
				forecastTerminus: forecast,
				plan: 15,
				dailyValue: 28m
			);

			await repo.AddAsync(rental);

			var handler = new ReturnMotorcycleHandler(repo);
			var result = await handler.HandleAsync(new ReturnMotorcycleRequest
			{
				RentalId = "loc3",
				ReturnDate = forecast.AddDays(2)
			});

			result.TotalValue.Should().Be((17 * 28m) + (2 * 50m));
			result.Description.Should().Contain("Additional");
		}
	}
}
