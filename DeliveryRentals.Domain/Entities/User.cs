
namespace DeliveryRentals.Domain.Entities
{
	public class User
	{
		public string Id { get; private set; } = Guid.NewGuid().ToString();
		public string Username { get; private set; } = string.Empty; // CNPJ or "admin"
		public string PasswordHash { get; private set; } = string.Empty;
		public string Role { get; private set; } = "courier"; // or "admin"

		protected User() { }

		public User(string username, string passwordHash, string role)
		{
			Username = username;
			PasswordHash = passwordHash;
			Role = role;
		}
	}
}
