using DeliveryRentals.Application.UseCases.Motos;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryRentals.API.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/[controller]")]
public class MotorcycleController : ControllerBase
{
	private readonly RegisterMotoHandler _registerHandler;
	private readonly GetMotosHandler _getHandler;
	private readonly UpdateMotoPlateHandler _updateHandler;
	private readonly DeleteMotoHandler _deleteHandler;

	public MotorcycleController(
		IMotoRepository motoRepository,
		IEventPublisher eventPublisher,
		IRentalRepository rentalRepository)
	{
		_registerHandler = new RegisterMotoHandler(motoRepository, eventPublisher);
		_getHandler = new GetMotosHandler(motoRepository);
		_updateHandler = new UpdateMotoPlateHandler(motoRepository);
		_deleteHandler = new DeleteMotoHandler(motoRepository, rentalRepository);
	}

	/// <summary>
	/// Register a new motorcycle.
	/// </summary>
	/// <param name="request">Motorcycle details.</param>
	/// <returns>201 Created successfully</returns>
	[HttpPost]
	[Authorize(Policy = "AdminOnly")]
	[SwaggerOperation(Summary = "Register a new motorcycle", Description = "Registers a new motorcycle and publishes an event.")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Register([FromBody] RegisterMotoRequest request)
	{
		try
		{
			await _registerHandler.HandleAsync(request);
			return Created("", null);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Check all motorcycles, with optional filter by license plate.
	/// </summary>
	/// <param name="licensePlate">Filter by license plate</param>
	[HttpGet]
	[Authorize(Policy = "AdminOnly")]
	[SwaggerOperation(Summary = "Get all motorcycles", Description = "Returns all motorcycles, filtered by license plate if provided.")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll([FromQuery] string? licensePlate = null)
	{
		var result = await _getHandler.HandleAsync(new GetMotosQuery { LicensePlate = licensePlate });
		return Ok(result);
	}

	/// <summary>
	/// Updates a motorcycle's license plate.
	/// </summary>
	[HttpPut("{id}/plate")]
	[Authorize(Policy = "AdminOnly")]
	[SwaggerOperation(Summary = "Update motorcycle plate", Description = "Updates the license plate of an existing motorcycle.")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdatePlate(string id, [FromBody] string newPlate)
	{
		try
		{
			await _updateHandler.HandleAsync(new UpdateMotoPlateRequest
			{
				MotoId = id,
				NewLicensePlate = newPlate
			});

			return Ok(new { message = "Plate updated successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Remove a motorcycle if it has no rentals.
	/// </summary>
	[HttpDelete("{id}")]
	[Authorize(Policy = "AdminOnly")]
	[SwaggerOperation(Summary = "Delete a motorcycle", Description = "Removes a motorcycle if no rentals are linked to it.")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Delete(string id)
	{
		try
		{
			await _deleteHandler.HandleAsync(id);
			return Ok(new { message = "Moto removed successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}
