using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/prospect-presets")]
[Authorize]
public class ProspectPresetsController : ControllerBase
{
    private readonly IProspectListPresetService _presetService;

    public ProspectPresetsController(IProspectListPresetService presetService)
    {
        _presetService = presetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPresets()
    {
        try
        {
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            var presets = await _presetService.GetPresetsAsync(organizationId);
            return Ok(presets);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPreset(int id)
    {
        try
        {
            var preset = await _presetService.GetPresetByIdAsync(id);
            if (preset == null)
                return NotFound(new { error = $"Preset with ID {id} not found" });

            return Ok(preset);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePreset([FromBody] ProspectListPreset preset)
    {
        try
        {
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            preset.OrganizationId = organizationId;
            preset.IsSystemPreset = false;

            var presetId = await _presetService.CreatePresetAsync(preset);
            return CreatedAtAction(nameof(GetPreset), new { id = presetId }, new { id = presetId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePreset(int id, [FromBody] ProspectListPreset preset)
    {
        try
        {
            if (id != preset.Id)
                return BadRequest(new { error = "ID mismatch" });

            var existingPreset = await _presetService.GetPresetByIdAsync(id);
            if (existingPreset == null)
                return NotFound(new { error = $"Preset with ID {id} not found" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (existingPreset.OrganizationId != organizationId && !existingPreset.IsSystemPreset)
                return Forbid();

            if (existingPreset.IsSystemPreset)
                return BadRequest(new { error = "System presets cannot be modified" });

            preset.IsSystemPreset = false;
            preset.OrganizationId = organizationId;

            await _presetService.UpdatePresetAsync(preset);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePreset(int id)
    {
        try
        {
            var preset = await _presetService.GetPresetByIdAsync(id);
            if (preset == null)
                return NotFound(new { error = $"Preset with ID {id} not found" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (preset.OrganizationId != organizationId && !preset.IsSystemPreset)
                return Forbid();

            if (preset.IsSystemPreset)
                return BadRequest(new { error = "System presets cannot be deleted" });

            await _presetService.DeletePresetAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}/properties")]
    public async Task<IActionResult> GetPropertiesByPreset(int id)
    {
        try
        {
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            var properties = await _presetService.GetPropertiesByPresetAsync(id, organizationId);
            return Ok(properties);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}