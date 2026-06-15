using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard/recent-work")]
public class RecentWorkController : ControllerBase
{
    private const int MaxLimit = 20;
    private readonly IRecentWorkService _recentWorkService;

    public RecentWorkController(IRecentWorkService recentWorkService)
    {
        _recentWorkService = recentWorkService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecentWorkSummaryDto>>> GetRecent(
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var normalizedLimit = Math.Clamp(limit, 1, MaxLimit);
        var summaries = await _recentWorkService.GetRecentAsync(userId.Value, normalizedLimit, cancellationToken);
        return Ok(summaries);
    }

    [HttpPost]
    public async Task<IActionResult> Record(
        RecordRecentWorkRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        await _recentWorkService.RecordAccessAsync(userId.Value, request, cancellationToken);
        return NoContent();
    }
}
