using Microsoft.AspNetCore.Authentication.Cookies;
using REIstacks.Application.Interfaces;
namespace REIStacks.Api.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtCookieMiddleware> _logger;

        public JwtCookieMiddleware(RequestDelegate next, ILogger<JwtCookieMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            _logger.LogInformation("Entering JwtCookieMiddleware for path: {Path}", context.Request.Path);
            try
            {
                // Check if the access_token cookie exists
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    _logger.LogInformation("Found 'access_token' cookie");

                    // Validate the token and get principal
                    var principal = tokenService.GetPrincipalFromExpiredToken(token);

                    // Create a new identity using the cookie authentication scheme
                    var identity = new System.Security.Claims.ClaimsIdentity(
                        principal.Claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    // Set the user with the new identity
                    context.User = new System.Security.Claims.ClaimsPrincipal(identity);

                    var claimsInfo = string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    _logger.LogInformation("Set user principal with claims: {Claims}", claimsInfo);
                }
                else
                {
                    _logger.LogInformation("No 'access_token' cookie found in the request for path: {Path}", context.Request.Path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in JwtCookieMiddleware while processing path: {Path}", context.Request.Path);
                // Continue processing even if token validation fails
            }

            // Call the next middleware in the pipeline
            await _next(context);
            _logger.LogInformation("Exiting JwtCookieMiddleware for path: {Path}", context.Request.Path);
        }
    }

    // Extension method to make it easier to add the middleware
    public static class JwtCookieMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtCookieAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieMiddleware>();
        }
    }
}