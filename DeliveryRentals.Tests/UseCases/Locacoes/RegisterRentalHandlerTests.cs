using DeliveryRentals.Application.UseCases.Rentals;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Persistence.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Locacoes
{
	public class RegisterRentalHandlerTests
	{
		[Fact]
		public async Task Must_register_rental_with_courier_using_license_A()
		{
			// Arrange
			var context = DbContextTestHelper.CreateInMemoryContext();
			var courierRepo = new EfCourierRepository(context);
			var motoRepo = new EfMotoRepository(context);
			var rentalRepo = new EfRentalRepository(context);

			var courier = new Courier("e1", "João", "123", DateTime.Today.AddYears(-30), "CNH123", "A");
			await courierRepo.AddAsync(courier);

			var moto = new Motorcycle("m1", 2024, "CG 160", "ABC-1234");
			await motoRepo.AddAsync(moto);

			var handler = new RegisterRentalHandler(courierRepo, motoRepo, rentalRepo);

			var request = new RegisterRentalRequest
			{
				CourierId = "e1",
				MotoId = "m1",
				Plan = 7
			};

			var today = DateTime.UtcNow.Date;
			var expectedStartDate = today.AddDays(1);

			// Act
			var response = await handler.HandleAsync(request);

			// Assert - locação persistida
			var rentals = await rentalRepo.GetAllAsync();

			rentals.Should().ContainSingle(l =>
				l.CourierId == "e1" &&
				l.MotoId == "m1" &&
				l.Plan == 7 &&
				l.DailyValue == 30m &&
				l.StartDate == expectedStartDate &&
				l.ForecastDateEnd == expectedStartDate.AddDays(7));

			// Assert - resposta correta
			response.Should().NotBeNull();
			response.MotoId.Should().Be("m1");
			response.CourierId.Should().Be("e1");
			response.Plan.Should().Be("7 days");
			response.DailyValue.Should().Be("$ 30");
			response.TotalExpectedValue.Should().Be("$ 210");
			response.StartDate.Should().Be(expectedStartDate);
			response.ForecastDateEnd.Should().Be(expectedStartDate.AddDays(7));
		}

		[Fact]
		public async Task Must_fail_if_courier_does_not_have_license_A()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var courierRepo = new EfCourierRepository(context);
			var motoRepo = new EfMotoRepository(context);
			var rentalRepo = new EfRentalRepository(context);

			var courier = new Courier("e2", "Maria", "999", DateTime.Today.AddYears(-28), "CNH999", "B");
			await courierRepo.AddAsync(courier);
			await motoRepo.AddAsync(new Motorcycle("m2", 2024, "PCX", "XYZ-9999"));

			var handler = new RegisterRentalHandler(courierRepo, motoRepo, rentalRepo);

			var request = new RegisterRentalRequest
			{
				CourierId = "e2",
				MotoId = "m2",
				Plan = 15
			};

			var act = async () => await handler.HandleAsync(request);

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Courier must have CNH type A");
		}
	}
}
