using DeliveryRentals.Infrastructure.Messaging;

namespace DeliveryRentals.Tests.Fake
{
	public class FakeEventPublisher : IEventPublisher
	{
		public List<object> PublishedEvents { get; } = new();

		public Task PublishAsync<T>(T @event) where T : class
		{
			PublishedEvents.Add(@event!);
			return Task.CompletedTask;
		}
	}
}
