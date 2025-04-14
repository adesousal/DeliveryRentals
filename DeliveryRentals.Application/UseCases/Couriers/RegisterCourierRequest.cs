
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DeliveryRentals.Application.UseCases.Couriers
{
	public class RegisterCourierRequest
	{
		[Required]
		public string Id { get; set; } = string.Empty;

		[Required]
		public string Name { get; set; } = string.Empty;

		[Required]
		public string Cnpj { get; set; } = string.Empty;

		[Required]
		public DateTime BirthDate { get; set; }

		[Required]
		public string CnhNumber { get; set; } = string.Empty;

		[Required]
		public string CnhType { get; set; } = string.Empty;

		public IFormFile? CnhImage { get; set; } = default!;
	}
}
