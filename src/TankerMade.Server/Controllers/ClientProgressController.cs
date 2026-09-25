using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services;

namespace TankerMade.Server.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/client-progress")]
public class ClientProgressController : ControllerBase
{
    private readonly ICommissionPublicationService _publications;

    public ClientProgressController(ICommissionPublicationService publications)
    {
        _publications = publications;
    }

    [HttpGet("{token}")]
    public async Task<ActionResult<ClientProgressSnapshotDto>> Read(string token, CancellationToken cancellationToken)
    {
        var snapshot = await _publications.GetByTokenAsync(token, cancellationToken);
        return snapshot == null ? NotFound() : Ok(snapshot);
    }

    [HttpGet("{token}/photos/{assetId:guid}")]
    public async Task<ActionResult> Photo(string token, Guid assetId, CancellationToken cancellationToken)
    {
        var photo = await _publications.OpenPhotoAsync(token, assetId, cancellationToken);
        if (photo == null)
        {
            return NotFound();
        }

        return File(photo.Content, photo.ContentType);
    }
}
