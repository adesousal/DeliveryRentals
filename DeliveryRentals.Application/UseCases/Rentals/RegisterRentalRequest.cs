
using System.ComponentModel.DataAnnotations;

namespace DeliveryRentals.Application.UseCases.Rentals
{
	public class RegisterRentalRequest
	{
		[Required]
		public string CourierId { get; set; } = string.Empty;

		[Required]
		public string MotoId { get; set; } = string.Empty;

		[Required]
		public int Plan { get; set; } // 7, 15, 30, 45, 50
	}
}
