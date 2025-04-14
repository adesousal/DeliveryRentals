using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Repositories;
using FluentAssertions;

namespace DeliveryRentals.Tests.UseCases.Motos
{
	public class GetMotosHandlerTests
	{
		[Fact]
		public async Task Must_return_all_motorcycles_when_no_filter_is_applied()
		{
			var repo = new InMemoryMotoRepository();
			await repo.AddAsync(new Motorcycle("1", 2023, "CG", "AAA-1111"));
			await repo.AddAsync(new Motorcycle("2", 2024, "Biz", "BBB-2222"));

			var handler = new GetMotosHandler(repo);

			var result = await handler.HandleAsync(new GetMotosQuery());

			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task Must_return_only_motorcycle_with_license_plate_matching_filter()
		{
			var repo = new InMemoryMotoRepository();
			await repo.AddAsync(new Motorcycle("1", 2023, "CG", "AAA-1111"));
			await repo.AddAsync(new Motorcycle("2", 2024, "Biz", "ZZZ-9999"));

			var handler = new GetMotosHandler(repo);

			var result = await handler.HandleAsync(new GetMotosQuery { LicensePlate = "zzz" });

			result.Should().ContainSingle(m => m.LicensePlate == "ZZZ-9999");
		}

		[Fact]
		public async Task Must_return_motorcycle_by_license_plate_case_insensitive()
		{
			// Arrange
			var repo = new InMemoryMotoRepository();
			var moto = new Motorcycle("moto-1", 2024, "Start", "XYZ-1234");
			await repo.AddAsync(moto);

			// Act
			var result = await repo.GetByLicensePlateAsync("xyz-1234");

			// Assert
			result.Should().NotBeNull();
			result!.Id.Should().Be("moto-1");
		}

		[Fact]
		public async Task Must_return_null_when_license_plate_does_not_exist()
		{
			// Arrange
			var repo = new InMemoryMotoRepository();

			// Act
			var result = await repo.GetByLicensePlateAsync("non-existent");

			// Assert
			result.Should().BeNull();
		}
	}
}
