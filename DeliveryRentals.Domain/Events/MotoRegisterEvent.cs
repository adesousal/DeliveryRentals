
namespace DeliveryRentals.Domain.Events
{
	public class MotoRegisterEvent
	{
		public string Id { get; set; } = string.Empty;
		public int Year { get; set; }
		public string Model { get; set; } = string.Empty;
		public string LicensePlate { get; set; } = string.Empty;
	}
}
