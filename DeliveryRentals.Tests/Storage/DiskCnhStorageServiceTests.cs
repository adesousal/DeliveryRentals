using DeliveryRentals.Infrastructure.Storage;
using FluentAssertions;

namespace DeliveryRentals.Tests.Storage
{
	public class DiskCnhStorageServiceTests
	{
		private const string TestFolder = "CnhImages_TempTest";

		[Fact]
		public async Task Deve_salvar_cnh_em_disco_e_retornar_caminho()
		{
			// Arrange
			if (Directory.Exists(TestFolder))
				Directory.Delete(TestFolder, true);

			var service = new DiskCnhStorageService(TestFolder);
			var courierId = "e999";
			var fileName = "cnh.png";

			var expectedFilePath = Path.Combine(TestFolder, $"{courierId}_{fileName}");

			await using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

			// Act
			var returnedPath = await service.SaveAsync(courierId, stream, fileName);

			// Assert
			File.Exists(expectedFilePath).Should().BeTrue();
			returnedPath.Should().Be(expectedFilePath);

			// Cleanup
			Directory.Delete(TestFolder, true);
		}
	}
}
