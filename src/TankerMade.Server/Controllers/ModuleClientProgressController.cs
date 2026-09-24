using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/projects/{projectId:guid}/client-progress")]
public class ModuleClientProgressController : ControllerBase
{
    private readonly ICommissionPublicationService _publications;

    public ModuleClientProgressController(ICommissionPublicationService publications)
    {
        _publications = publications;
    }

    [HttpGet("workshop")]
    public async Task<ActionResult<CommissionWorkshopDto>> Workshop(
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var workshop = await _publications.GetWorkshopAsync(userId.Value, moduleKey, projectId, cancellationToken);
        return workshop == null ? NotFound() : Ok(workshop);
    }

    [HttpPut("terms")]
    public async Task<ActionResult> SaveTerms(
        string moduleKey,
        Guid projectId,
        UpdateCommissionTermsRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return ToStatus(await _publications.SaveTermsAsync(userId.Value, moduleKey, projectId, request, cancellationToken));
    }

    [HttpPost("link")]
    public async Task<ActionResult<CommissionCommandResult>> ReissueLink(
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return ToAction(await _publications.ReissueLinkAsync(userId.Value, moduleKey, projectId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CommissionCommandResult>> Publish(
        string moduleKey,
        Guid projectId,
        PublishClientProgressRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return ToAction(await _publications.PublishAsync(userId.Value, moduleKey, projectId, request, cancellationToken));
    }

    [HttpPost("revisions")]
    public async Task<ActionResult<CommissionCommandResult>> AddRevision(
        string moduleKey,
        Guid projectId,
        AddClientProgressRevisionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return ToAction(await _publications.AddRevisionAsync(userId.Value, moduleKey, projectId, request, cancellationToken));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke(string moduleKey, Guid projectId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _publications.RevokeAsync(userId.Value, moduleKey, projectId, cancellationToken);
        return result.Status switch
        {
            CommissionCommandStatus.ModuleInactive => Forbid(),
            CommissionCommandStatus.NotFound => NotFound(),
            _ => NoContent()
        };
    }

    [HttpGet]
    public async Task<ActionResult<ClientProgressSnapshotDto>> Preview(
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var snapshot = await _publications.GetPreviewAsync(userId.Value, moduleKey, projectId, cancellationToken);
        return snapshot == null ? NotFound() : Ok(snapshot);
    }

    private ActionResult<CommissionCommandResult> ToAction(CommissionCommandResult result)
    {
        return result.Status switch
        {
            CommissionCommandStatus.ModuleInactive => Forbid(),
            CommissionCommandStatus.NotFound => NotFound(),
            _ => Ok(result)
        };
    }

    private ActionResult ToStatus(CommissionCommandResult result)
    {
        return result.Status switch
        {
            CommissionCommandStatus.ModuleInactive => Forbid(),
            CommissionCommandStatus.NotFound => NotFound(),
            _ => NoContent()
        };
    }
}
