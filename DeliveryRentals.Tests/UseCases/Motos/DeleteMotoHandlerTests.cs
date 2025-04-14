using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class DeleteMotoHandlerTests
	{
		[Fact]
		public async Task Must_remove_motorcycle_when_there_are_no_rentals()
		{
			var motoRepo = new InMemoryMotoRepository();
			var rentalRepo = new InMemoryRentalRepository();

			await motoRepo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			await handler.HandleAsync("m1");

			var moto = await motoRepo.GetByIdAsync("m1");
			moto.Should().BeNull();
		}

		[Fact]
		public async Task Must_fail_if_motorcycle_has_active_rentals()
		{
			var motoRepo = new InMemoryMotoRepository();
			var rentalRepo = new InMemoryRentalRepository();

			await motoRepo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));

			// Simula locação já registrada
			rentalRepo.AddRental("m1", "courier1");

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			var act = async () => await handler.HandleAsync("m1");

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Cannot delete moto with existing rentals");
		}

		[Fact]
		public async Task Must_fail_when_motorcycle_does_not_exist()
		{
			var motoRepo = new InMemoryMotoRepository();
			var rentalRepo = new InMemoryRentalRepository();

			var handler = new DeleteMotoHandler(motoRepo, rentalRepo);

			var act = async () => await handler.HandleAsync("does-not-exist");

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Moto not found");
		}
	}
}
