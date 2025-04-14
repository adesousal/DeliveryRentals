
namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface IEventRepository
	{
		Task SaveEventAsync(string json);
	}
}
