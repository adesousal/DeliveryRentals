
namespace DeliveryRentals.Application.UseCases.Motos
{
	public class MotoDto
	{
		public string Id { get; set; } = string.Empty;
		public int Year { get; set; }
		public string Model { get; set; } = string.Empty;
		public string LicensePlate { get; set; } = string.Empty;
	}
}
