
namespace DeliveryRentals.Infrastructure.Storage
{
	public interface ICnhStorageService
	{
		Task<string> SaveAsync(string courierId, Stream fileStream, string fileName);
	}
}
