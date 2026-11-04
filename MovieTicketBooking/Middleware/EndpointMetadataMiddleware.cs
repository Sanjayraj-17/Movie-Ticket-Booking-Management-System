
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBooking.Middleware
{
    public class EndpointMetadataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EndpointMetadataMiddleware> _logger;

        // Constructor
        public EndpointMetadataMiddleware(
            RequestDelegate next,
            ILogger<EndpointMetadataMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Executes for every HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(
                "========== ENDPOINT METADATA ==========");

            // Get the selected API endpoint
            var endpoint = context.GetEndpoint();

            // Display request details
            _logger.LogInformation(
                "Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (endpoint is null)
            {
                _logger.LogWarning("No endpoint selected.");
            }
            else
            {
                // Display endpoint name
                _logger.LogInformation(
                    "Endpoint: {Endpoint}",
                    endpoint.DisplayName);

                // Check AllowAnonymous attribute
                var allowAnonymous =
                    endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;

                // Get authorization attributes
                var authorizeData =
                    endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>();

                _logger.LogInformation(
                    "AllowAnonymous: {AllowAnonymous}",
                    allowAnonymous);

                _logger.LogInformation(
                    "Authorization metadata count: {Count}",
                    authorizeData.Count);
            }

            // Continue to the next middleware
            await _next(context);
        }
    }
}
