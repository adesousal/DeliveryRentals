
namespace DeliveryRentals.Application.UseCases.Rentals
{
	public class ReturnMotorcycleRequest
	{
		public string RentalId { get; set; } = string.Empty;
		public DateTime ReturnDate { get; set; }
	}
}
