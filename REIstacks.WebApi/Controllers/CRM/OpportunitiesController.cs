using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/opportunities")]
[Authorize]
public class OpportunitiesController : TenantController
{
    private readonly IOpportunityService _svc;
    public OpportunitiesController(IOpportunityService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _svc.GetAllAsync(OrgId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var opp = await _svc.GetByIdAsync(id, OrgId);
        if (opp == null) return NotFound();
        return Ok(opp);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Opportunity dto)
    {
        dto.OrganizationId = OrgId;
        var id = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Opportunity dto)
    {
        if (id != dto.Id) return BadRequest();
        dto.OrganizationId = OrgId;
        var ok = await _svc.UpdateAsync(dto);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteAsync(id, OrgId);
        return ok ? NoContent() : NotFound();
    }

    // PUT api/opportunities/5/assign
    [HttpPut("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] Guid userProfileId)
    {
        var ok = await _svc.AssignToUserAsync(id, userProfileId, OrgId);
        return ok ? NoContent() : NotFound();
    }

    // PUT api/opportunities/5/unassign
    [HttpPut("{id:int}/unassign")]
    public async Task<IActionResult> Unassign(int id)
    {
        var ok = await _svc.UnassignAsync(id, OrgId);
        return ok ? NoContent() : NotFound();
    }
}
