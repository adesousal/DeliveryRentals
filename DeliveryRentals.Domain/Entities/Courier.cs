
namespace DeliveryRentals.Domain.Entities
{
	public class Courier
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string Cnpj { get; private set; }
		public DateTime BirthDate { get; private set; }
		public string CnhNumber { get; private set; }
		public string CnhType { get; private set; } // A, B ou A+B
		public string? CnhImagePath { get; private set; }

		public Courier(string id, string name, string cnpj, DateTime birthDate, string cnhNumber, string cnhType)
		{
			Id = id;
			Name = name;
			Cnpj = cnpj;
			BirthDate = birthDate;
			CnhNumber = cnhNumber;
			CnhType = cnhType;
		}

		public void UpdateCnhImage(string imagePath)
		{
			CnhImagePath = imagePath;
		}
	}
}
