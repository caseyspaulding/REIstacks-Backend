using Azure.Communication.Identity;
using Microsoft.AspNetCore.Mvc;

namespace REIstacks.Api.Controllers.Communications
{
    [ApiController]
    [Route("api/calling")]
    public class CallingController : ControllerBase
    {
        private readonly string _acsConnectionString;
        private readonly ILogger<CallingController> _logger;

        public CallingController(IConfiguration config, ILogger<CallingController> logger)
        {
            _logger = logger;

            try
            {
                _acsConnectionString = config["AzureCommunicationService"]
                    ?? throw new ArgumentNullException("AzureCommunicationService config not found");

                _logger.LogInformation("CallingController initialized with ACS connection string");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to initialize CallingController. Configuration error.");
                throw; // Rethrow to prevent controller initialization
            }
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetAcsToken()
        {
            _logger.LogInformation("Token request received at: {time}", DateTimeOffset.UtcNow);

            try
            {
                _logger.LogDebug("Creating CommunicationIdentityClient");
                var client = new CommunicationIdentityClient(_acsConnectionString);

                _logger.LogDebug("Creating new user identity");
                var userResponse = await client.CreateUserAsync();
                _logger.LogInformation("User created with ID: {userId}", userResponse.Value.Id);

                _logger.LogDebug("Getting token for user with VoIP scope");
                var tokenResponse = await client.GetTokenAsync(
                    userResponse.Value,
                    scopes: new[] { CommunicationTokenScope.VoIP });

                _logger.LogInformation("Token generated successfully. Expires: {expiresOn}", tokenResponse.Value.ExpiresOn);

                return Ok(new
                {
                    userId = userResponse.Value.Id,
                    token = tokenResponse.Value.Token,
                    expiresOn = tokenResponse.Value.ExpiresOn
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ACS token");

                // Return a more detailed error in development, sanitized in production
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    return StatusCode(500, new
                    {
                        error = "Failed to generate token",
                        details = ex.Message,
                        stackTrace = ex.StackTrace
                    });
                }

                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        // Add a diagnostic endpoint to verify the controller is working
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping request received");
            return Ok(new
            {
                status = "ok",
                timestamp = DateTimeOffset.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
    }
}