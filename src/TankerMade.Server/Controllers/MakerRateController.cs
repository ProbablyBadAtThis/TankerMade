using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/account/maker-rate")]
public class MakerRateController : ControllerBase
{
    private readonly ICommissionPublicationService _publications;

    public MakerRateController(ICommissionPublicationService publications)
    {
        _publications = publications;
    }

    [HttpGet]
    public async Task<ActionResult<MakerRateDto>> Get(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var rate = await _publications.GetMakerRateAsync(userId.Value, cancellationToken);
        return rate == null ? NotFound() : Ok(rate);
    }

    [HttpPut]
    public async Task<ActionResult<MakerRateDto>> Set(MakerRateDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var rate = await _publications.SetMakerRateAsync(userId.Value, request.TargetHourlyRate, cancellationToken);
            return rate == null ? NotFound() : Ok(rate);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
