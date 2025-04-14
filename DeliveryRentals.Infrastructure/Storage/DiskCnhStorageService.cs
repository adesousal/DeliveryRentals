
namespace DeliveryRentals.Infrastructure.Storage
{
	public class DiskCnhStorageService : ICnhStorageService
	{
		private readonly string _rootPath;

		public DiskCnhStorageService(string? rootPath = null)
		{
			_rootPath = rootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "CnhImages");
		}

		public async Task<string> SaveAsync(string courierId, Stream fileStream, string fileName)
		{
			Directory.CreateDirectory(_rootPath);
			var path = Path.Combine(_rootPath, $"{courierId}_{fileName}");

			using var output = File.Create(path);
			await fileStream.CopyToAsync(output);

			return path;
		}
	}
}
