using DeliveryRentals.Application.Services;
using System.Net;
using System.Text.Json;

namespace DeliveryRentals.API.Middleware
{
	/// <summary>
	/// Middleware that captures unhandled exceptions and returns a friendly error response.
	/// It also logs the error using the application's logging service.
	/// </summary>
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Executes the middleware logic to catch and handle unexpected exceptions.
		/// Logs the error and returns a generic error message to the client.
		/// </summary>
		/// <param name="context">The current HTTP context.</param>
		/// <param name="logger">The logging service used to persist error information.</param>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
