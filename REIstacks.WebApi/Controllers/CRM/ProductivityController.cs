using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using System.Security.Claims;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/productivity")]
[Authorize]
public class ProductivityController : TenantController
{
    private readonly IProductivityService _svc;

    public ProductivityController(IProductivityService svc)
        => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var orgId = OrgId;                                 // from TenantController
        var profile = Guid.Parse(User.FindFirstValue("sub")); // or however you model profile IDs
        var metrics = await _svc.GetProductivityAsync(orgId, profile);
        return Ok(metrics);
    }
}
