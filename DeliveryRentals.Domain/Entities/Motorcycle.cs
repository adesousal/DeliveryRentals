
namespace DeliveryRentals.Domain.Entities
{
	public class Motorcycle
	{
		public string Id { get; private set; }
		public int Year { get; private set; }
		public string Model { get; private set; }
		public string LicensePlate { get; private set; }

		public Motorcycle(string id, int year, string model, string licensePlate)
		{
			Id = id;
			Year = year;
			Model = model;
			LicensePlate = licensePlate;
		}

		public void UpdatePlate(string newPlate)
		{
			LicensePlate = newPlate;
		}
	}
}
