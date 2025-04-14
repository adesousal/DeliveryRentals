using DeliveryRentals.Application.UseCases.Rentals;
using DeliveryRentals.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryRentals.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalController : ControllerBase
{
	private readonly RegisterRentalHandler _registerHandler;
	private readonly ReturnMotorcycleHandler _devolverHandler;

	public RentalController(
		ICourierRepository courierRepo,
		IMotoRepository motoRepo,
		IRentalRepository rentalRepo)
	{
		_registerHandler = new RegisterRentalHandler(courierRepo, motoRepo, rentalRepo);
		_devolverHandler = new ReturnMotorcycleHandler(rentalRepo);
	}

	/// <summary>
	/// Make a new lease.
	/// </summary>
	[HttpPost]
	[Authorize(Policy = "CourierOnly")]
	[SwaggerOperation(Summary = "Start new rental", Description = "Creates a new rental. Only couriers with CNH A are allowed.")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RentMotorcycle([FromBody] RegisterRentalRequest request)
	{
		try
		{
			var response = await _registerHandler.HandleAsync(request);
			return Created("", response);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Register the return of the motorcycle and calculate the total value.
	/// </summary>
	[HttpPut("{id}/return")]
	[Authorize(Policy = "CourierOnly")]
	[SwaggerOperation(Summary = "Return rented motorcycle", Description = "Registers the return date and calculates total cost, including penalties or extras.")]
	[ProducesResponseType(typeof(ReturnMotoResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ReturnMotorcycle(string id, [FromBody] DateTime returnDate)
	{
		try
		{
			var result = await _devolverHandler.HandleAsync(new ReturnMotorcycleRequest
			{
				RentalId = id,
				ReturnDate = returnDate
			});

			return Ok(result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}