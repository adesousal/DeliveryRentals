
namespace DeliveryRentals.Application.UseCases.Rentals
{
	public class RegisterRentalResponse
	{
		public string MotoId { get; set; }

		public string CourierId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime ForecastDateEnd { get; set; }

		public string Plan { get; set; }

		public string DailyValue { get; set; }

		public string TotalExpectedValue { get; set; }
	}
}
