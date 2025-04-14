
namespace DeliveryRentals.Infrastructure.Messaging
{
	public interface IEventPublisher
	{
		Task PublishAsync<T>(T @event) where T : class;
	}
}
