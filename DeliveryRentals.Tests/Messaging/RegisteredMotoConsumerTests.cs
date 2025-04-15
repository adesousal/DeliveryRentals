using DeliveryRentals.Domain.Events;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Persistence.Context;
using DeliveryRentals.Persistence.Repositories;
using FluentAssertions;
using System.Text.Json;

namespace DeliveryRentals.Tests.Messaging
{
	public class RegisteredMotoConsumerTests
	{
		[Fact]
		public async Task Must_save_event_when_year_is_2024()
		{
			// Arrange
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repo = new EfEventRepository(context);
			var consumer = new RegisteredMotoConsumer(repo);

			var @event = new MotoRegisterEvent
			{
				Id = "m1",
				Year = 2024,
				Model = "Start",
				LicensePlate = "XYZ-2024"
			};

			var json = JsonSerializer.Serialize(@event);

			// Act
			await consumer.HandleAsync(json);

			// Assert
			var events = await repo.GetEvents();
			events.Should().ContainSingle(e => e.Contains("XYZ-2024"));
		}

		[Fact]
		public async Task Must_not_save_event_when_year_is_not_2024()
		{
			// Arrange
			var context = DbContextTestHelper.CreateInMemoryContext();
			var repo = new EfEventRepository(context);
			var consumer = new RegisteredMotoConsumer(repo);

			var @event = new MotoRegisterEvent
			{
				Id = "m2",
				Year = 2023,
				Model = "Biz",
				LicensePlate = "ZZZ-2023"
			};

			var json = JsonSerializer.Serialize(@event);

			// Act
			await consumer.HandleAsync(json);

			// Assert
			var events = await repo.GetEvents();
			events.Should().BeEmpty();
		}
	}
}
