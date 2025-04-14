
namespace DeliveryRentals.Infrastructure.Repositories
{
	public class InMemoryEventRepository : IEventRepository
	{
		private readonly List<string> _events = new();

		public Task SaveEventAsync(string json)
		{
			_events.Add(json);
			return Task.CompletedTask;
		}

		// To view stored events (ex: GET /events)
		public IEnumerable<string> GetEvents() => _events;
	}
}
