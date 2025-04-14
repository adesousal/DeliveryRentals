using DeliveryRentals.Domain.Events;
using DeliveryRentals.Infrastructure.Repositories;
using System.Text.Json;

namespace DeliveryRentals.Infrastructure.Messaging
{
	public class RegisteredMotoConsumer : IEventConsumer
	{
		private readonly IEventRepository _eventRepository;

		public RegisteredMotoConsumer(IEventRepository eventoRepository)
		{
			_eventRepository = eventoRepository;
		}

		public async Task HandleAsync(string messageJson)
		{
			var motoEvent = JsonSerializer.Deserialize<MotoRegisterEvent>(messageJson);
			if (motoEvent?.Year == 2024)
			{
				await _eventRepository.SaveEventAsync(messageJson);
			}
		}
	}
}
