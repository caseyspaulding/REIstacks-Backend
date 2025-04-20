using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using System.Security.Claims;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/skiptrace")]
[Authorize]
public class SkipTraceController : TenantController
{
    private readonly ISkipTraceService _svc;
    public SkipTraceController(ISkipTraceService svc) => _svc = svc;

    // POST api/skiptrace
    [HttpPost]
    public async Task<IActionResult> Start([FromBody] int[] contactIds)
    {
        var orgId = User.FindFirstValue("organization_id");
        if (string.IsNullOrEmpty(orgId)) return Unauthorized();

        var activity = await _svc.StartSkipTraceAsync(contactIds, orgId);
        return CreatedAtAction(nameof(GetById), new { id = activity.Id }, activity);
    }

    // GET api/skiptrace
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var orgId = User.FindFirstValue("organization_id");
        if (string.IsNullOrEmpty(orgId)) return Unauthorized();

        var list = await _svc.GetActivitiesAsync(orgId);
        return Ok(list);
    }

    // GET api/skiptrace/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orgId = User.FindFirstValue("organization_id");
        if (string.IsNullOrEmpty(orgId)) return Unauthorized();

        var activity = await _svc.GetActivityByIdAsync(id, orgId);
        if (activity == null) return NotFound();
        return Ok(activity);
    }

    // DELETE api/skiptrace/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var orgId = User.FindFirstValue("organization_id");
        if (string.IsNullOrEmpty(orgId)) return Unauthorized();

        var ok = await _svc.CancelActivityAsync(id, orgId);
        return ok ? NoContent() : NotFound();
    }
}