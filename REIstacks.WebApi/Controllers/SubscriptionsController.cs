using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstack.Domain.Models;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces;
using REIstacks.Infrastructure.Data;
using System.Security.Claims;

namespace reistacks_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ITokenService _jwtService;
    private readonly IActivityLogger _activityLogger;
    private readonly AppDbContext _context;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(
        ITokenService jwtService,
        IActivityLogger activityLogger,
        AppDbContext context,
        ILogger<SubscriptionsController> logger)
    {
        _jwtService = jwtService;
        _activityLogger = activityLogger;
        _context = context;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateSubscription([FromBody] SubscriptionUpdateRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid or missing authentication token.");
            }

            if (userId != request.UserId)
            {
                return Unauthorized("User ID does not match token.");
            }

            // Find user's organization
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId);

            if (organization == null)
            {
                return NotFound("Organization not found.");
            }

            // ✅ Ensure SubscriptionStatus is a valid enum value
            if (!Enum.TryParse(request.SubscriptionStatus, true, out SubscriptionStatus parsedStatus))
            {
                return BadRequest(new { error = $"Invalid subscription status: {request.SubscriptionStatus}" });
            }

            // Update or insert a StripeSubscription record
            var existingSubscription = await _context.StripeSubscriptions
                .FirstOrDefaultAsync(s => s.StripeSubscriptionId == request.SubscriptionId);

            if (existingSubscription != null)
            {
                existingSubscription.Status = parsedStatus;
                existingSubscription.CurrentPeriodStart = request.CurrentPeriodStart;
                existingSubscription.CurrentPeriodEnd = request.CurrentPeriodEnd;
                existingSubscription.CancelAtPeriodEnd = request.CancelAtPeriodEnd;
                existingSubscription.UpdatedAt = DateTime.UtcNow;
                existingSubscription.TrialStart = request.TrialStart;
                existingSubscription.TrialEnd = request.TrialEnd;
            }
            else
            {
                var newSubscription = new StripeSubscription
                {
                    OrganizationId = request.OrganizationId,
                    StripeSubscriptionId = request.SubscriptionId,
                    PlanId = request.PlanId,
                    Status = parsedStatus, // ✅ Using parsedStatus instead of direct conversion
                    CurrentPeriodStart = request.CurrentPeriodStart,
                    CurrentPeriodEnd = request.CurrentPeriodEnd,
                    CancelAtPeriodEnd = request.CancelAtPeriodEnd,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    TrialStart = request.TrialStart,
                    TrialEnd = request.TrialEnd
                };

                _context.StripeSubscriptions.Add(newSubscription);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError("Subscription update failed: {Message}", ex.Message);
            return BadRequest(new { error = "Subscription update failed." });
        }
    }

    // **Extracts User ID from JWT Token**
    private string GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim?.Value;
    }
}
