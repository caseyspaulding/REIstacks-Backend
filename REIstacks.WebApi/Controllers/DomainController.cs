using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces;
using System.Security.Claims;

namespace reistacks_api.Controllers
{
    [ApiController]
    [Route("api/domains")]
    [Authorize] // Ensure only authenticated users can access these routes
    public class DomainController : ControllerBase
    {
        private readonly IDomainVerificationService _domainVerificationService;

        public DomainController(IDomainVerificationService domainVerificationService)
        {
            _domainVerificationService = domainVerificationService;
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenRequest request)
        {
            try
            {
                // 🔥 Get the organization ID & subdomain from JWT claims
                var organizationId = User.FindFirstValue("organization_id");
                var subdomain = User.FindFirstValue("subdomain");

                if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(subdomain))
                {
                    return Unauthorized(new { error = "Invalid authentication token" });
                }

                // 🔥 Ensure that the domain belongs to the logged-in organization
                string token = await _domainVerificationService.GenerateVerificationTokenAsync(
                    organizationId, request.Domain);

                return Ok(new
                {
                    token,
                    verificationRecords = new[]
                    {
                        new
                        {
                            type = "TXT",
                            name = "_reistacks-verification." + request.Domain,
                            value = token
                        },
                        new
                        {
                            type = "CNAME",
                            name = request.Domain,
                            value = subdomain + ".reistacks.com" // ✅ Ensures correct tenant subdomain
                        }
                    },
                    instructions = "Add these DNS records to verify your domain ownership."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyDomain([FromBody] VerifyDomainRequest request)
        {
            try
            {
                var organizationId = User.FindFirstValue("organization_id");

                if (string.IsNullOrEmpty(organizationId))
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                bool isVerified = await _domainVerificationService.VerifyDomainAsync(
                    organizationId, request.Domain, request.Token);

                if (!isVerified)
                {
                    return BadRequest(new { error = "Domain verification failed" });
                }

                return Ok(new { message = "Domain verified successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingVerifications()
        {
            try
            {
                var organizationId = User.FindFirstValue("organization_id");

                if (string.IsNullOrEmpty(organizationId))
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                var pendingVerifications = await _domainVerificationService.GetPendingVerificationsAsync(organizationId);

                return Ok(pendingVerifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class GenerateTokenRequest
    {
        public string Domain { get; set; }
    }

    public class VerifyDomainRequest
    {
        public string Domain { get; set; }
        public string Token { get; set; }
    }
}
