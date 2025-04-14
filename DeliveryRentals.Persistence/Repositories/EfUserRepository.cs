using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRentals.Persistence.Repositories
{
	public class EfUserRepository : IUserRepository
	{
		private readonly AppDbContext _context;

		public EfUserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<User?> GetByUsernameAsync(string username)
		{
			return await _context.Set<User>()
				.FirstOrDefaultAsync(u => u.Username == username);
		}

		public async Task AddAsync(User user)
		{
			await _context.AddAsync(user);
			await _context.SaveChangesAsync();
		}
	}
}
