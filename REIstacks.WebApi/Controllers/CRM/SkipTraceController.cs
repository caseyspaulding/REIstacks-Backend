using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/skiptrace")]
[Authorize]
public class SkipTraceController : TenantController
{
    private readonly ISkipTraceService _svc;
    public SkipTraceController(ISkipTraceService svc) => _svc = svc;

    // GET api/skiptrace?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetActivities(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _svc.GetActivitiesAsync(page, pageSize, OrgId);
        return Ok(new { data = items, total, page, pageSize });
    }

    // GET api/skiptrace/{id}/breakdown
    [HttpGet("{id:int}/breakdown")]
    public async Task<IActionResult> GetBreakdown(int id)
    {
        try
        {
            var breakdown = await _svc.GetBreakdownAsync(id, OrgId);
            return Ok(breakdown);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Activity not found" });
        }
    }

    // POST api/skiptrace
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SkipTraceActivity dto)
    {
        dto.OrganizationId = OrgId;
        var id = await _svc.CreateActivityAsync(dto);
        return CreatedAtAction(nameof(GetBreakdown), new { id }, dto);
    }
}