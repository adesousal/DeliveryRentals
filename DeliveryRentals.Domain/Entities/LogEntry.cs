
namespace DeliveryRentals.Domain.Entities
{
	public class LogEntry
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Level { get; set; } = "Info";
		public string Message { get; set; } = string.Empty;
		public string? Exception { get; set; }
		public string? User { get; set; }
		public string? Role { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}
