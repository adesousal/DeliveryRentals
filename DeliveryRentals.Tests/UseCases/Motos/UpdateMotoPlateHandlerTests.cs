using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Persistence.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class UpdateMotoPlateHandlerTests
	{
		[Fact]
		public async Task Must_update_motorcycle_license_plate_when_new_plate_is_valid()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repo = new EfMotoRepository(context);
			await repo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));

			var handler = new UpdateMotoPlateHandler(repo);
			var request = new UpdateMotoPlateRequest
			{
				MotoId = "m1",
				NewLicensePlate = "ZZZ-9999"
			};

			await handler.HandleAsync(request);

			var moto = await repo.GetByIdAsync("m1");
			moto.Should().NotBeNull();
			moto!.LicensePlate.Should().Be("ZZZ-9999");
		}

		[Fact]
		public async Task Must_throw_error_if_motorcycle_does_not_exist()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repo = new EfMotoRepository(context);
			var handler = new UpdateMotoPlateHandler(repo);

			var request = new UpdateMotoPlateRequest
			{
				MotoId = "dos-not-exists",
				NewLicensePlate = "ZZZ-9999"
			};

			var act = async () => await handler.HandleAsync(request);

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Moto not found");
		}

		[Fact]
		public async Task Must_throw_error_if_new_license_plate_is_already_in_use()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repo = new EfMotoRepository(context);
			await repo.AddAsync(new Motorcycle("m1", 2023, "CG", "AAA-1111"));
			await repo.AddAsync(new Motorcycle("m2", 2023, "Biz", "ZZZ-9999"));

			var handler = new UpdateMotoPlateHandler(repo);

			var request = new UpdateMotoPlateRequest
			{
				MotoId = "m1",
				NewLicensePlate = "ZZZ-9999"
			};

			var act = async () => await handler.HandleAsync(request);

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("License plate already in use");
		}
	}
}
