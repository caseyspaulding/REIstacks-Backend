using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstack.Domain.Models;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data;
using System.Security.Claims;

namespace reistacks_api.Controllers;

[Route("api/organization")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly ITokenService _jwtService;
    private readonly IActivityLogger _activityLogger;
    private readonly AppDbContext _context;
    private readonly ILogger<OrganizationController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationController(
        ITokenService jwtService,
        IUnitOfWork unitOfWork,
         IActivityLogger activityLogger,
        AppDbContext context,
        ILogger<OrganizationController> logger)
    {
        _jwtService = jwtService;
        _activityLogger = activityLogger;
        _context = context;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 🔍 Checks if a hostname (subdomain or custom domain) is registered
    /// </summary>
    [HttpGet("org-check")]  // ✅ FIX: Corrected route
    public async Task<IActionResult> CheckOrganization([FromQuery] string hostname)
    {
        if (string.IsNullOrEmpty(hostname))
        {
            return BadRequest(new { error = "Hostname is required." });
        }

        // 🔥 Detect if it's a subdomain or a custom domain
        var rootDomain = "reistacks.com"; // ✅ Change to match your root domain
        bool isCustomDomain = !hostname.EndsWith($".{rootDomain}") && hostname != rootDomain;

        Organization org = null;

        if (isCustomDomain)
        {
            // ✅ Check by custom domain
            org = await _context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.CustomDomain == hostname);
        }
        else
        {
            // ✅ Check by subdomain
            string subdomain = hostname.Split('.')[0];
            org = await _context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Subdomain == subdomain);
        }

        if (org == null)
        {
            return NotFound(new { error = "Organization not found." });
        }

        // ✅ Select only necessary fields
        return Ok(new
        {
            id = org.Id,
            name = org.Name,
            subdomain = org.Subdomain,
            customDomain = org.CustomDomain,
            logoUrl = org.LogoUrl,
            primaryColor = org.ThemeColor
        });
    }

    [HttpGet("by-subdomain/{subdomain}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySubdomain(string subdomain)
    {

        // Find the organization by subdomain
        var organization = await _unitOfWork.Organizations.GetBySubdomainAsync(subdomain);
        if (organization == null)
            return NotFound(new { error = "Organization not found" });

        // Check if user belongs to this organization


        // Return organization details
        return Ok(new
        {
            id = organization.Id,
            name = organization.Name,
            subdomain = organization.Subdomain,
            customDomain = organization.CustomDomain,
            themeColor = organization.ThemeColor,
            logoUrl = organization.LogoUrl
        });
    }

    /// <summary>
    /// 🔍 Gets organization by authenticated user
    /// </summary>
    [HttpGet("by-user")]
    public async Task<IActionResult> GetOrganizationByUser()
    {
        var userId = GetUserIdFromToken();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid authentication token." });
        }

        // ✅ Convert userId (string) to Guid
        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            return BadRequest(new { error = "Invalid user ID format." });
        }

        // ✅ Fetch organization where user is the owner
        var organization = await _context.Organizations
            .AsNoTracking()
            .Where(o => o.OwnerId == userGuid)
            .Select(o => new  // ✅ Only return necessary fields
            {
                id = o.Id,
                name = o.Name,
                subdomain = o.Subdomain,
                customDomain = o.CustomDomain,
                logoUrl = o.LogoUrl,
                primaryColor = o.ThemeColor
            })
            .FirstOrDefaultAsync();

        if (organization == null)
        {
            return NotFound(new { error = "Organization not found." });
        }

        return Ok(organization);
    }


    /// <summary>
    /// 🔐 Extracts User ID from JWT Token
    /// </summary>
    private string GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim?.Value;
    }
}
