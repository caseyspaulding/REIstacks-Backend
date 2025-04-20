using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.User;

namespace REIstacks.Api.Controllers.Organizations;

[ApiController]
[Route("api/team")]
public class TeamInvitationController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;

    public TeamInvitationController(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IGoogleAuthService googleAuthService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _googleAuthService = googleAuthService;
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinTeam([FromBody] TeamJoinRequest request)
    {
        try
        {
            // Validate Google token
            var validationResult = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, request.Nonce);
            if (!validationResult.IsValid)
                return BadRequest(new { error = "Invalid Google token" });

            var email = validationResult.Claims["email"].ToString();
            var googleId = validationResult.Claims["sub"].ToString();
            var name = validationResult.Claims.ContainsKey("name") ?
                validationResult.Claims["name"].ToString() : email;

            // Verify invitation exists and matches email
            var invitation = await _unitOfWork.Invitations.GetByIdAsync(request.InviteId);
            if (invitation == null || invitation.Status != "pending")
                return BadRequest(new { error = "Invalid or expired invitation" });

            if (invitation.Email.ToLower() != email.ToLower())
                return BadRequest(new { error = "Email doesn't match invitation" });

            // Get organization
            var organization = await _unitOfWork.Organizations.GetByIdAsync(invitation.OrganizationId);
            if (organization == null)
                return BadRequest(new { error = "Organization not found" });

            // Create profile for team member
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                ExternalId = googleId,
                ExternalProvider = "Google",
                OrganizationId = organization.Id,
                Role = Role.Member, // Use role from invitation if needed
                IsSetupComplete = true, // Team members skip setup
                LastLogin = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserProfiles.AddAsync(profile);

            // Create external auth entry
            var externalAuth = new ExternalAuth
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                Provider = "Google",
                ExternalId = googleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExternalAuths.AddAsync(externalAuth);

            // Update invitation status
            invitation.Status = "accepted";
            _unitOfWork.Invitations.Update(invitation);

            await _unitOfWork.CompleteAsync();

            // Set auth cookies
            _tokenService.SetAuthCookies(HttpContext, profile, organization);

            // Return success with profile info
            return Ok(new
            {
                profile = new
                {
                    id = profile.Id,
                    email = profile.Email,
                    name = profile.Name,
                    role = profile.Role.ToString(),
                    isSetupComplete = true,
                    organizationId = profile.OrganizationId,
                    subdomain = organization.Subdomain
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
