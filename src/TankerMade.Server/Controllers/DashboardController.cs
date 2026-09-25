using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardOverviewService _dashboardOverviewService;

    public DashboardController(IDashboardOverviewService dashboardOverviewService)
    {
        _dashboardOverviewService = dashboardOverviewService;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewDto>> GetOverview(
        [FromQuery] string? moduleKey = null,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var overview = await _dashboardOverviewService.GetOverviewAsync(userId.Value, moduleKey, cancellationToken);
        return Ok(overview);
    }
}
