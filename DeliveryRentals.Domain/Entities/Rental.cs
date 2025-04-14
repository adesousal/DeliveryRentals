
using System.ComponentModel.DataAnnotations;

namespace DeliveryRentals.Domain.Entities
{
	public class Rental
	{
		protected Rental() { }

		public string Id { get; private set; }
		
		public string MotoId { get; private set; }
		
		public string CourierId { get; private set; }
		
		public DateTime StartDate { get; private set; }
		
		public DateTime EndDate { get; private set; }
		
		public DateTime ForecastDateEnd { get; private set; }
		
		public int Plan { get; private set; }

		public decimal DailyValue { get; private set; }

		public DateTime? ReturnDate { get; private set; }

		public Rental(string id, string motoId, string courierId, DateTime start, DateTime end, DateTime forecastTerminus, int plan, decimal dailyValue)
		{
			Id = id;
			MotoId = motoId;
			CourierId = courierId;
			StartDate = start;
			EndDate = end;
			ForecastDateEnd = forecastTerminus;
			Plan = plan;
			DailyValue = dailyValue;
		}

		public void RegistrarDevolucao(DateTime data)
		{
			ReturnDate = data;
		}
	}
}
