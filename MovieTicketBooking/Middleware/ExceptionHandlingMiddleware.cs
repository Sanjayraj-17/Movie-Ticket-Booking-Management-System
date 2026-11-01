using System.Net;
using System.Text.Json;

namespace MovieTicketBooking.Middleware
{
    // Centralized middleware to intercept unhandled exceptions and return clean JSON responses
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue to the next middleware / controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An unhandled exception occurred during HTTP request processing.");

                // Return a structured JSON error response
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            // Map specific exception types to appropriate HTTP status codes
            if (exception is ArgumentException argumentEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                message = argumentEx.Message;
            }
            else if (exception is InvalidOperationException invalidOpEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                message = invalidOpEx.Message;
            }
            else if (exception is KeyNotFoundException keyNotFoundEx)
            {
                statusCode = (int)HttpStatusCode.NotFound; // 404
                message = keyNotFoundEx.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError; // 500
                message = "An unexpected error occurred on the server. Please try again later.";
            }

            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                statusCode = statusCode,
                message = message
            };

            var jsonResult = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(jsonResult);
        }
    }
}
