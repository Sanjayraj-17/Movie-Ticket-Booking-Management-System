
using Microsoft.AspNetCore.Http;

namespace MovieTicketBooking.Middleware
{
    public class AuthenticationDiagnosticsMiddleware
    {
        // Passes the request to the next middleware
        private readonly RequestDelegate _next;

        // Used to display information in the console
        private readonly ILogger<AuthenticationDiagnosticsMiddleware> _logger;

        // Constructor
        public AuthenticationDiagnosticsMiddleware(
            RequestDelegate next,
            ILogger<AuthenticationDiagnosticsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // This method executes for every request
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(
                "========== AUTHENTICATION RESULT ==========");

            // Check whether the user is authenticated
            _logger.LogInformation(
                "Is Authenticated: {IsAuthenticated}",
                context.User.Identity?.IsAuthenticated);

            // Display authentication type
            _logger.LogInformation(
                "Authentication Type: {AuthenticationType}",
                context.User.Identity?.AuthenticationType);

            // Display all claims from the authenticated user
            foreach (var claim in context.User.Claims)
            {
                _logger.LogInformation(
                    "Claim Type: {ClaimType}, Claim Value: {ClaimValue}",
                    claim.Type,
                    claim.Value);
            }

            // Continue to the next middleware
            await _next(context);
        }
    }
}
