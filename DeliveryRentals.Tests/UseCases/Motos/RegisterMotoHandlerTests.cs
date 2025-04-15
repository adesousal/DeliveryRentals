using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Events;
using DeliveryRentals.Persistence.Repositories;
using DeliveryRentals.Tests.Fake;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class RegisterMotoHandlerTests
	{
		[Fact]
		public async Task Must_register_motorcycle_when_license_plate_is_valid()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repository = new EfMotoRepository(context);

			var publisher = new FakeEventPublisher();
			var handler = new RegisterMotoHandler(repository, publisher);

			var request = new RegisterMotoRequest
			{
				Id = Guid.NewGuid().ToString(),
				Year = 2024,
				Model = "CG 160 Fan",
				LicensePlate = "ABC-1234"
			};

			await handler.HandleAsync(request);

			var motos = await repository.GetAllAsync();
			motos.Should().ContainSingle(m => m.LicensePlate == "ABC-1234");

			publisher.PublishedEvents
				.Should().ContainSingle(e => ((MotoRegisterEvent)e).LicensePlate == "ABC-1234");
		}

		[Fact]
		public async Task Must_throw_error_when_license_plate_is_already_in_use()
		{
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repository = new EfMotoRepository(context);
			var publisher = new FakeEventPublisher();

			var handler = new RegisterMotoHandler(repository, publisher);

			var request1 = new RegisterMotoRequest
			{
				Id = "1",
				Year = 2023,
				Model = "Biz",
				LicensePlate = "XYZ-9876"
			};

			var request2 = new RegisterMotoRequest
			{
				Id = "2",
				Year = 2024,
				Model = "PCX",
				LicensePlate = "XYZ-9876"
			};

			await handler.HandleAsync(request1);

			var action = async () => await handler.HandleAsync(request2);

			await action.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("License plate already registered");
		}
	}
}
