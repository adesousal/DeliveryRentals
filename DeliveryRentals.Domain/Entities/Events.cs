namespace DeliveryRentals.Domain.Entities
{
	public class Events
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string? Dados { get; set; }
		public DateTime? Timestamp { get; set; } = DateTime.UtcNow;

		protected Events() { }

		public Events (string json){
			Dados = json;
		}
	}
}
