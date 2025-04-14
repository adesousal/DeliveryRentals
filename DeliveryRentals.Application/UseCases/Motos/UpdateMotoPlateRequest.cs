
namespace DeliveryRentals.Application.UseCases.Motos
{
	public class UpdateMotoPlateRequest
	{
		public string MotoId { get; set; } = string.Empty;
		public string NewLicensePlate { get; set; } = string.Empty;
	}
}
