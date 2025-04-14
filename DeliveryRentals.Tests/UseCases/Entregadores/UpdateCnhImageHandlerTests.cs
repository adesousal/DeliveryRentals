using DeliveryRentals.Application.UseCases.Couriers;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Storage;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Couriers
{
	public class UpdateCnhImageHandlerTests
	{
		[Fact]
		public async Task Must_save_photo_to_existing_courier_record()
		{
			var repo = new InMemoryCourierRepository();
			var courier = new Courier("e1", "John", "123", DateTime.Today.AddYears(-30), "CNH1", "A");
			await repo.AddAsync(courier);

			var storage = new InMemoryCnhStorageService();
			var handler = new UpdateCnhImageHandler(repo, storage);

			var fileName = "cnh.png";
			using var stream = new MemoryStream(new byte[] { 1, 2, 3 }); // Simula imagem

			await handler.HandleAsync("e1", stream, fileName);

			var saved = await repo.GetByIdAsync("e1");
			saved!.CnhImagePath.Should().NotBeNull();
			saved.CnhImagePath.Should().Contain("e1_cnh.png");
		}

		[Fact]
		public async Task Must_throw_error_for_invalid_format()
		{
			var repo = new InMemoryCourierRepository();
			await repo.AddAsync(new Courier("e1", "João", "123", DateTime.Today, "CNH1", "A"));

			var storage = new InMemoryCnhStorageService();
			var handler = new UpdateCnhImageHandler(repo, storage);

			var fileName = "documento.pdf"; // inválido
			using var stream = new MemoryStream(new byte[] { 1 });

			var act = async () => await handler.HandleAsync("e1", stream, fileName);

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Invalid file format.");
		}

		[Fact]
		public async Task Must_throw_error_for_nonexistent_courier()
		{
			var repo = new InMemoryCourierRepository();
			var storage = new InMemoryCnhStorageService();
			var handler = new UpdateCnhImageHandler(repo, storage);

			using var stream = new MemoryStream(new byte[] { 1 });

			var act = async () => await handler.HandleAsync("e404", stream, "cnh.png");

			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Courier not found.");
		}
	}

	// In-memory image storage simulation
	public class InMemoryCnhStorageService : ICnhStorageService
	{
		private readonly Dictionary<string, byte[]> _files = new();

		public Task<string> SaveAsync(string courierId, Stream fileStream, string fileName)
		{
			var path = $"{courierId}_{fileName}";
			using var ms = new MemoryStream();
			fileStream.CopyTo(ms);
			_files[path] = ms.ToArray();
			return Task.FromResult(path);
		}
	}
}
