using DeliveryRentals.Application.Services;
using System.Text.Json;

namespace DeliveryRentals.API.Middleware
{
	/// <summary>
	/// Middleware responsible for logging requests and handling unhandled exceptions.
	/// It also ignores logging for static resources and Swagger endpoints.
	/// </summary>
	public class RequestLoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public RequestLoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Handles the incoming HTTP request, logs exceptions,
		/// and filters out static resources and Swagger calls.
		/// </summary>
		/// <param name="context">The current HTTP context.</param>
		/// <param name="logger">The logging service to persist error logs.</param>
		public async Task Invoke(HttpContext context, LoggingService logger)
		{
			try
			{
				// Ignore static files and Swagger calls
				var path = context.Request.Path.Value?.ToLower();
				if (path != null &&
					(path.StartsWith("/swagger") || path.Contains(".js") || path.Contains(".css") || path.Contains(".ico")))
				{
					await _next(context);
					return;
				}

				await _next(context);
			}
			catch (Exception ex)
			{
				context.Response.ContentType = "application/json";

				var userId = context.User?.FindFirst("sub")?.Value;
				var name = context.User?.Identity?.Name;
				var userDisplay = userId != null && name != null ? $"{userId} - {name}" : null;

				int statusCode;
				string messageToClient;

				switch (ex)
				{
					case InvalidOperationException:
						statusCode = StatusCodes.Status400BadRequest;
						messageToClient = ex.Message;
						break;

					case UnauthorizedAccessException:
						statusCode = StatusCodes.Status403Forbidden;
						messageToClient = "Access denied.";
						break;

					default:
						statusCode = StatusCodes.Status500InternalServerError;
						messageToClient = "An unexpected error has occurred. Please try again later.";
						break;
				}

				context.Response.StatusCode = statusCode;
				await logger.Error("Error caught in middleware", ex, userDisplay);

				var result = JsonSerializer.Serialize(new { error = messageToClient });
				await context.Response.WriteAsync(result);
			}
		}
	}
}
