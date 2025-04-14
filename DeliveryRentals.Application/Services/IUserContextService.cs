namespace DeliveryRentals.Application.Services
{
	public interface IUserContextService
	{
		string? UserId { get; }
		string? Name { get; }
		string? Role { get; }
		bool HasUser { get; }
	}
}
