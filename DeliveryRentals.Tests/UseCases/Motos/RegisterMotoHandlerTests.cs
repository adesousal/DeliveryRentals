using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Events;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Infrastructure.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class RegisterMotoHandlerTests
	{
		[Fact]
		public async Task Must_register_motorcycle_when_license_plate_is_valid()
		{
			// Arrange
			var repository = new InMemoryMotoRepository();
			var eventConsumer = new TestMotoCadastradaConsumer();
			var eventPublisher = new InMemoryEventPublisher(eventConsumer);

			var handler = new RegisterMotoHandler(repository, eventPublisher);

			var request = new RegisterMotoRequest
			{
				Id = Guid.NewGuid().ToString(),
				Year = 2024,
				Model = "CG 160 Fan",
				LicensePlate = "ABC-1234"
			};

			// Act
			await handler.HandleAsync(request);

			// Assert
			var motos = await repository.GetAllAsync();
			motos.Should().ContainSingle(m => m.LicensePlate == "ABC-1234");

			eventConsumer.Events.Should().ContainSingle(e => e.LicensePlate == "ABC-1234");
		}

		[Fact]
		public async Task Must_throw_error_when_license_plate_is_already_in_use()
		{
			// Arrange
			var repository = new InMemoryMotoRepository();
			var publisher = new InMemoryEventPublisher(new TestMotoCadastradaConsumer());

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

			// Act
			var action = async () => await handler.HandleAsync(request2);

			// Assert
			await action.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("License plate already registered");
		}
	}

	// 🔧 Test consumer (simulates event listener)
	public class TestMotoCadastradaConsumer : IEventConsumer
	{
		public List<MotoRegisterEvent> Events { get; } = new();

		public Task HandleAsync(string messageJson)
		{
			var @event = System.Text.Json.JsonSerializer.Deserialize<MotoRegisterEvent>(messageJson);
			if (@event != null)
				Events.Add(@event);

			return Task.CompletedTask;
		}
	}
}
