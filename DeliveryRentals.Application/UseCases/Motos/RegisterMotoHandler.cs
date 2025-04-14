using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Domain.Events;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Infrastructure.Repositories;

namespace DeliveryRentals.Application.UseCases.Motos
{
	public class RegisterMotoHandler
	{
		private readonly IMotoRepository _repository;
		private readonly IEventPublisher _eventPublisher;

		public RegisterMotoHandler(IMotoRepository repository, IEventPublisher eventPublisher)
		{
			_repository = repository;
			_eventPublisher = eventPublisher;
		}

		public async Task HandleAsync(RegisterMotoRequest request)
		{
			if (await _repository.ExistsByPlateAsync(request.LicensePlate))
				throw new InvalidOperationException("License plate already registered");

			var moto = new Motorcycle(request.Id, request.Year, request.Model, request.LicensePlate);
			await _repository.AddAsync(moto);

			var registerEvent = new MotoRegisterEvent
			{
				Id = moto.Id,
				Year = moto.Year,
				Model = moto.Model,
				LicensePlate = moto.LicensePlate
			};

			await _eventPublisher.PublishAsync(registerEvent);
		}
	}
}
