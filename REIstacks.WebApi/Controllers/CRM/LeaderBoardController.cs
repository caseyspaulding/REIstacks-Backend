using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;


namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/sales")]
[Authorize]
public class LeaderBoardController : TenantController
{
    private readonly ISalesLeaderBoardService _svc;
    public LeaderBoardController(ISalesLeaderBoardService svc) => _svc = svc;

    [HttpGet("leaderboard")]
    public async Task<IActionResult> LeaderBoard()
    {
        var board = await _svc.GetLeaderBoardAsync(OrgId);
        return Ok(board);
    }
}
