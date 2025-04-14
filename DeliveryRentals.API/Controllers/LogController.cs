using DeliveryRentals.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryRentals.API.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Policy = "AdminOnly")]
	[ApiController]
	public class LogController : ControllerBase
	{
		private readonly ILogRepository _repo;

		public LogController(ILogRepository repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] string? user = null,
											 [FromQuery] DateTime? from = null,
											 [FromQuery] DateTime? to = null,
											 [FromQuery] int take = 100,
											 [FromQuery] int skip = 0)
		{
			var logs = await _repo.SearchAsync(user, from, to, take, skip);
			return Ok(logs);
		}
	}
}
