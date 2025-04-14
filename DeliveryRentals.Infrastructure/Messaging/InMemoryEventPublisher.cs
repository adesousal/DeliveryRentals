using System.Text.Json;

namespace DeliveryRentals.Infrastructure.Messaging
{
	public class InMemoryEventPublisher : IEventPublisher
	{
		private readonly IEventConsumer _consumer;

		public InMemoryEventPublisher(IEventConsumer consumer)
		{
			_consumer = consumer;
		}

		public async Task PublishAsync<T>(T @event) where T : class
		{
			var json = JsonSerializer.Serialize(@event);
			await _consumer.HandleAsync(json);
		}
	}
}
