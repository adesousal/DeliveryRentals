
namespace DeliveryRentals.Infrastructure.Messaging
{
	public interface IEventConsumer
	{
		Task HandleAsync(string messageJson);
	}
}
