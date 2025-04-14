using DeliveryRentals.Domain.Entities;

namespace DeliveryRentals.Infrastructure.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetByUsernameAsync(string username);
		Task AddAsync(User user);
	}
}
