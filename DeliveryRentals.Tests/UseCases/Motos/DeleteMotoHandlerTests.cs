using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class DeleteMotoHandlerTests
	{
		[Fact]
		public async Task Must_remove_motorcycle_when_there_are_no_rentals()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var motoRepo = new EfMotoRepository(context);
			var rentalRepo = new EfRentalRepository(context);

			await motoRepo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			await handler.HandleAsync("m1");

			var moto = await motoRepo.GetByIdAsync("m1");
			moto.Should().BeNull();
		}

		[Fact]
		public async Task Must_fail_if_motorcycle_has_active_rentals()
		{
			// Arrange
			var context = DbContextTestHelper.CreateInMemoryContext();
			var motoRepo = new EfMotoRepository(context);
			var rentalRepo = new EfRentalRepository(context);

			await motoRepo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));

			await rentalRepo.AddAsync(new Rental(
				id: "r1",
				motoId: "m1",
				courierId: "courier1",
				start: DateTime.UtcNow,
				end: DateTime.UtcNow.AddDays(7),
				forecastTerminus: DateTime.UtcNow.AddDays(7),
				plan: 7,
				dailyValue: 30m
			));

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			// Act
			var act = async () => await handler.HandleAsync("m1");

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Cannot delete moto with existing rentals");
		}

		[Fact]
		public async Task Must_fail_when_motorcycle_does_not_exist()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var motoRepo = new EfMotoRepository(context);
			var rentalRepo = new EfRentalRepository(context);

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			var act = async () => await handler.HandleAsync("does-not-exist");

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Moto not found");
		}
	}
}
