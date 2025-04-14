using DeliveryRentals.Application.UseCases.Couriers;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryRentals.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourierController : ControllerBase
{
	private readonly RegisterCourierHandler _registerHandler;
	private readonly UpdateCnhImageHandler _imageHandler;

	public CourierController(
		ICourierRepository repository,
		ICnhStorageService storage)
	{
		_registerHandler = new RegisterCourierHandler(repository, storage);
		_imageHandler = new UpdateCnhImageHandler(repository, storage);
	}

	/// <summary>
	/// Register a new courier.
	/// </summary>
	/// <param name="request">Courier details</param>
	[HttpPost]	
	[SwaggerOperation(Summary = "Register a new courier", Description = "Adds a new courier to the system with CNH validation.")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Register([FromForm] RegisterCourierRequest request)
	{
		try
		{
			await _registerHandler.HandleAsync(request);
			
			return Ok(new { message = "Registered courier successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Updates the courier's driver's license image.
	/// </summary>
	/// <param name="id">Courier ID</param>
	/// <param name="file">Image archive (png or bmp)</param>
	[HttpPost("{id}/cnh")]
	[Authorize(Roles = "courier")]
	[SwaggerOperation(Summary = "Upload CNH image", Description = "Updates courier's CNH image. Accepted formats: .png or .bmp.")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UploadCNH(string id, IFormFile file)
	{
		try
		{
			using var stream = file.OpenReadStream();
			await _imageHandler.HandleAsync(id, stream, file.FileName);
			return Ok(new { message = "CNH image updated successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}