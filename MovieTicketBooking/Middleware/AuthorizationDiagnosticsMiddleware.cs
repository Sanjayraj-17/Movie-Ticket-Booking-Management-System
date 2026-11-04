
using Microsoft.AspNetCore.Http;

namespace MovieTicketBooking.Middleware
{
    public class AuthorizationDiagnosticsMiddleware
    {
        // Passes the request to the next middleware
        private readonly RequestDelegate _next;

        // Used to display logs
        private readonly ILogger<AuthorizationDiagnosticsMiddleware> _logger;

        // Constructor
        public AuthorizationDiagnosticsMiddleware(
            RequestDelegate next,
            ILogger<AuthorizationDiagnosticsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Executes during each HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(
                "========== AUTHORIZATION PROCESS ==========");

            // Continue to the next middleware
            await _next(context);

            // Display the final HTTP response status
            _logger.LogInformation(
                "Final Response Status: {StatusCode}",
                context.Response.StatusCode);
        }
    }
}
