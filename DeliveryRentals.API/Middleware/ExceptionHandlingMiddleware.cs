using DeliveryRentals.Application.Services;
using System.Net;
using System.Text.Json;

namespace DeliveryRentals.API.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, LoggingService logger)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				var statusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.StatusCode = statusCode;
				context.Response.ContentType = "application/json";

				var userId = context.User?.FindFirst("sub")?.Value;
				var name = context.User?.Identity?.Name;
				var userDisplay = userId != null && name != null ? $"{userId} - {name}" : null;

				await logger.Error("Unhandled error", ex, userDisplay);

				var result = JsonSerializer.Serialize(new
				{
					error = "An unexpected error has occurred. Please try again later."
				});

				await context.Response.WriteAsync(result);
			}
		}
	}
}
