using DeliveryRentals.Application.UseCases.Couriers;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Storage;
using DeliveryRentals.Persistence.Context;
using DeliveryRentals.Persistence.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DeliveryRentals.Tests.UseCases.Couriers
{
	public class RegisterCourierHandlerTests
	{
		private EfCourierRepository _repo = new EfCourierRepository(DbContextTestHelper.CreateInMemoryContext());
		private DiskCnhStorageService _cnhStorage = new DiskCnhStorageService();

		[Fact]
		public async Task Must_register_dcourier_and_save_image()
		{
			// Arrange
			var repoMock = new Mock<ICourierRepository>();
			repoMock.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>())).ReturnsAsync(false);
			repoMock.Setup(x => x.ExistsByCnhAsync(It.IsAny<string>())).ReturnsAsync(false);

			var storageMock = new Mock<ICnhStorageService>();
			storageMock.Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
					   .ReturnsAsync("path/fake.png");

			var imageMock = new Mock<IFormFile>();
			imageMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));
			imageMock.Setup(f => f.FileName).Returns("cnh.png");

			var request = new RegisterCourierRequest
			{
				Id = "123",
				Name = "Anyone",
				Cnpj = "12345678000100",
				BirthDate = new DateTime(1990, 1, 1),
				CnhNumber = "CNH001",
				CnhType = "A",
				CnhImage = imageMock.Object
			};

			var handler = new RegisterCourierHandler(repoMock.Object, storageMock.Object);

			// Act
			await handler.HandleAsync(request);

			// Assert
			repoMock.Verify(x => x.AddAsync(It.IsAny<Courier>()), Times.Once);
			storageMock.Verify(x => x.SaveAsync("123", It.IsAny<Stream>(), "cnh.png"), Times.Once);
		}

		[Fact]
		public async Task Must_not_register_if_CNPJ_is_duplicate()
		{
			var handler = new RegisterCourierHandler(_repo, _cnhStorage);

			var request = new RegisterCourierRequest
			{
				Id = "e1",
				Name = "John",
				Cnpj = "123",
				BirthDate = DateTime.Today.AddYears(-19),
				CnhNumber = "CNH123",
				CnhType = "A"
			};

			await handler.HandleAsync(request);

			var action = async () => await handler.HandleAsync(request);

			await action.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("CNPJ already registered");
		}

		[Fact]
		public async Task Must_reject_duplicate_license_numbers()
		{
			// Arrange			
			var handler = new RegisterCourierHandler(_repo, _cnhStorage);

			var cnhDuplicated = "12345678";

			var courierExisting = new Courier("id1", "John", "11111111000191", new DateTime(1990, 1, 1), cnhDuplicated, "A");
			await _repo.AddAsync(courierExisting);

			var request = new RegisterCourierRequest
			{
				Id = "id2",
				Name = "Maria",
				Cnpj = "22222222000191",
				BirthDate = new DateTime(1995, 1, 1),
				CnhNumber = cnhDuplicated,
				CnhType = "A"
			};

			// Act + Assert
			var act = async () => await handler.HandleAsync(request);
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("CNH number already registered");
		}

		[Fact]
		public async Task Must_throw_error_when_license_type_is_invalid()
		{
			// Arrange
			var handler = new RegisterCourierHandler(_repo, _cnhStorage);

			var request = new RegisterCourierRequest
			{
				Id = "id3",
				Name = "Carl",
				Cnpj = "33333333000191",
				BirthDate = new DateTime(1985, 1, 1),
				CnhNumber = "CNH-003",
				CnhType = "C" // inválido
			};

			// Act + Assert
			var act = async () => await handler.HandleAsync(request);
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Invalid CNH type");
		}

		[Fact]
		public async Task Must_throw_error_for_future_birth_date()
		{
			var repo = new Mock<ICourierRepository>();
			repo.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>())).ReturnsAsync(false);
			repo.Setup(x => x.ExistsByCnhAsync(It.IsAny<string>())).ReturnsAsync(false);

			var storage = new Mock<ICnhStorageService>();

			var request = new RegisterCourierRequest
			{
				Id = "1",
				Name = "Future",
				Cnpj = "12345678000100",
				BirthDate = DateTime.UtcNow.AddDays(1),
				CnhNumber = "CNH123",
				CnhType = "A"
			};

			var handler = new RegisterCourierHandler(repo.Object, storage.Object);

			await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
		}

		[Fact]
		public async Task Must_fail_if_courier_is_underage()
		{
			var repo = new Mock<ICourierRepository>();
			repo.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>())).ReturnsAsync(false);
			repo.Setup(x => x.ExistsByCnhAsync(It.IsAny<string>())).ReturnsAsync(false);

			var storage = new Mock<ICnhStorageService>();

			var request = new RegisterCourierRequest
			{
				Id = "2",
				Name = "Minor",
				Cnpj = "12345678000199",
				BirthDate = DateTime.UtcNow.AddYears(-16),
				CnhNumber = "CNH456",
				CnhType = "B"
			};

			var handler = new RegisterCourierHandler(repo.Object, storage.Object);

			await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
		}
	}
}
