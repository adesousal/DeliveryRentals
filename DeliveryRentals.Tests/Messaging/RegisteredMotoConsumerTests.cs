using DeliveryRentals.Domain.Events;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Infrastructure.Repositories;
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
			var repo = new InMemoryEventRepository();
			var consumer = new RegisteredMotoConsumer(repo);

			var evento = new MotoRegisterEvent
			{
				Id = "m1",
				Year = 2024,
				Model = "Start",
				LicensePlate = "XYZ-2024"
			};

			var json = JsonSerializer.Serialize(evento);

			// Act
			await consumer.HandleAsync(json);

			// Assert
			repo.GetEvents().Should().ContainSingle(e => e.Contains("XYZ-2024"));
		}

		[Fact]
		public async Task Must_not_save_event_when_year_is_not_2024()
		{
			// Arrange
			var repo = new InMemoryEventRepository();
			var consumer = new RegisteredMotoConsumer(repo);

			var evento = new MotoRegisterEvent
			{
				Id = "m2",
				Year = 2023,
				Model = "Biz",
				LicensePlate = "ZZZ-2023"
			};

			var json = JsonSerializer.Serialize(evento);

			// Act
			await consumer.HandleAsync(json);

			// Assert
			repo.GetEvents().Should().BeEmpty();
		}
	}
}
