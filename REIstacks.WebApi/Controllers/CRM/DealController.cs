// REIstacks.Api.Controllers.CRM.DealsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Api.Controllers;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.Deals;
using System.Security.Cryptography;

[ApiController]
[Route("api/deals")]
[Authorize]
public class DealsController : TenantController
{
    private readonly IDealService _deals;

    public DealsController(IDealService deals)
    {
        _deals = deals;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _deals.GetDealsAsync(OrgId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var deal = await _deals.GetDealByIdAsync(id, OrgId);
        if (deal == null) return NotFound();
        return Ok(deal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Deal deal)
    {
        deal.OrganizationId = OrgId;
        var created = await _deals.CreateDealAsync(deal);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Deal deal)
    {
        if (id != deal.Id) return BadRequest();
        deal.OrganizationId = OrgId;
        var ok = await _deals.UpdateDealAsync(deal, OrgId);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _deals.DeleteDealAsync(id, OrgId);
        return ok ? NoContent() : NotFound();
    }

    // your enum‐driven endpoints…
    [HttpGet("types")]
    public ActionResult<IEnumerable<string>> GetDealTypes()
        => Ok(Enum.GetNames(typeof(DealType)));

    [HttpGet("statuses")]
    public ActionResult<IEnumerable<string>> GetDealStatuses()
        => Ok(Enum.GetNames(typeof(DealStatus)));
}
