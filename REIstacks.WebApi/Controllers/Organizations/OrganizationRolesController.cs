using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace REIstacks.Api.Controllers.Organizations;

[ApiController]
[Route("api/organizations/{organizationId}/roles")]
[Authorize]
public class OrganizationRolesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrganizationRolesController> _logger;

    public OrganizationRolesController(IUnitOfWork unitOfWork, ILogger<OrganizationRolesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // GET: api/organizations/{organizationId}/roles
    [HttpGet]
    public async Task<IActionResult> GetRoles(string organizationId)
    {
        // Verify user has access to this organization
        if (!await UserHasAccessToOrg(organizationId))
            return Forbid();

        var roles = await _unitOfWork.OrganizationRoles.GetByOrganizationIdAsync(organizationId);
        return Ok(roles);
    }

    // GET: api/organizations/{organizationId}/roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(string organizationId, int id)
    {
        if (!await UserHasAccessToOrg(organizationId))
            return Forbid();

        var role = await _unitOfWork.OrganizationRoles.GetByIdAsync(id);

        if (role == null || role.OrganizationId != organizationId)
            return NotFound();

        return Ok(role);
    }

    // POST: api/organizations/{organizationId}/roles
    [HttpPost]
    public async Task<IActionResult> CreateRole(string organizationId, [FromBody] OrganizationRoleRequest request)
    {
        if (!await UserHasAccessToOrg(organizationId, requiredRole: "Admin"))
            return Forbid();

        var role = new OrganizationRole
        {
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description,
            IsCustom = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.OrganizationRoles.AddAsync(role);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetRole), new { organizationId, id = role.Id }, role);
    }

    // PUT: api/organizations/{organizationId}/roles/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string organizationId, int id, [FromBody] OrganizationRoleRequest request)
    {
        if (!await UserHasAccessToOrg(organizationId, requiredRole: "Admin"))
            return Forbid();

        var role = await _unitOfWork.OrganizationRoles.GetByIdAsync(id);

        if (role == null || role.OrganizationId != organizationId)
            return NotFound();

        // Prevent modification of system roles
        if (!role.IsCustom)
            return BadRequest("System roles cannot be modified");

        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.OrganizationRoles.Update(role);
        await _unitOfWork.CompleteAsync();

        return Ok(role);
    }

    // DELETE: api/organizations/{organizationId}/roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string organizationId, int id)
    {
        if (!await UserHasAccessToOrg(organizationId, requiredRole: "Admin"))
            return Forbid();

        var role = await _unitOfWork.OrganizationRoles.GetByIdAsync(id);

        if (role == null || role.OrganizationId != organizationId)
            return NotFound();

        // Prevent deletion of system roles
        if (!role.IsCustom)
            return BadRequest("System roles cannot be deleted");

        // Check if any users are currently assigned this role
        var usersWithRole = await _unitOfWork.UserProfiles.GetByOrganizationRoleIdAsync(id);
        if (usersWithRole.Any())
            return BadRequest("Cannot delete a role that is assigned to users");

        await _unitOfWork.OrganizationRoles.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // Helper method to verify user access to an organization
    private async Task<bool> UserHasAccessToOrg(string organizationId, string requiredRole = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return false;

        var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(userId);
        if (userProfile == null || userProfile.OrganizationId != organizationId)
            return false;

        if (requiredRole != null)
        {
            var userRole = await _unitOfWork.OrganizationRoles.GetByIdAsync(userProfile.OrganizationRoleId ?? 0);
            return userRole != null && (userRole.Name == requiredRole || userRole.Name == "Owner");
        }

        return true;
    }
}

// Request DTO
public class OrganizationRoleRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }
}