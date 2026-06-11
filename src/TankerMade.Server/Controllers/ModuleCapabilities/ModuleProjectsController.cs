using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModuleProjects;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Controllers.ModuleCapabilities;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/projects")]
public class ModuleProjectsController : ControllerBase
{
    private const int DefaultPageSize = 50;
    private readonly IModuleService _moduleService;
    private readonly IModuleProjectCapabilityResolver _resolver;

    public ModuleProjectsController(IModuleService moduleService, IModuleProjectCapabilityResolver resolver)
    {
        _moduleService = moduleService;
        _resolver = resolver;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModuleProjectDto>>> GetAll(string moduleKey, [FromQuery] bool includeArchived = false, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetAllAsync(gate.UserId!.Value, includeArchived, page, pageSize));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ModuleProjectDto>>> Search(string moduleKey, [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(Array.Empty<ModuleProjectDto>());
        }

        return Ok(await gate.Handler!.SearchAsync(q, gate.UserId!.Value, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModuleProjectDto>> GetById(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var project = await gate.Handler!.GetByIdAsync(id, gate.UserId!.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ModuleProjectDto>> Create(string moduleKey, CreateModuleProjectRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var project = await gate.Handler!.CreateAsync(request, gate.UserId!.Value);
        return CreatedAtAction(nameof(GetById), new { moduleKey, id = project.Id }, project);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ModuleProjectDto>> Update(string moduleKey, Guid id, UpdateModuleProjectRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        request.Id = id;
        var project = await gate.Handler!.UpdateAsync(request, gate.UserId!.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<ActionResult<ModuleProjectDto>> Archive(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var project = await gate.Handler!.ArchiveAsync(id, gate.UserId!.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/reopen")]
    public async Task<ActionResult<ModuleProjectDto>> Reopen(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var project = await gate.Handler!.ReopenAsync(id, gate.UserId!.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.DeleteAsync(id, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/progress")]
    public async Task<ActionResult<ModuleProjectDto>> SetStepProgress(
        string moduleKey,
        Guid id,
        Guid stepId,
        UpdateModuleProjectStepProgressRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.SetStepProgressAsync(id, stepId, request, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/start")]
    public async Task<ActionResult<ModuleProjectDto>> StartTimer(
        string moduleKey,
        Guid id,
        Guid stepId,
        UpdateModuleProjectTimerRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.StartTimerAsync(id, stepId, request, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/pause")]
    public async Task<ActionResult<ModuleProjectDto>> PauseTimer(
        string moduleKey,
        Guid id,
        Guid stepId,
        UpdateModuleProjectTimerRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.PauseTimerAsync(id, stepId, request, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer")]
    public async Task<ActionResult<ModuleProjectDto>> SetTimer(
        string moduleKey,
        Guid id,
        Guid stepId,
        UpdateModuleProjectTimerRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.SetTimerAsync(id, stepId, request, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}/steps/{stepId:guid}/timer")]
    public async Task<ActionResult<ModuleProjectDto>> ResetTimer(string moduleKey, Guid id, Guid stepId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.ResetTimerAsync(id, stepId, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/inventory-links")]
    public async Task<ActionResult<ModuleProjectDto>> AddInventoryLink(
        string moduleKey,
        Guid id,
        CreateModuleProjectInventoryLinkRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            var project = await gate.Handler!.AddInventoryLinkAsync(id, request, gate.UserId!.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}/inventory-links/{linkId:guid}")]
    public async Task<ActionResult> RemoveInventoryLink(string moduleKey, Guid id, Guid linkId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.RemoveInventoryLinkAsync(id, linkId, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    private async Task<GateResult> ResolveGateAsync(string moduleKey)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return GateResult.Unauthorized();
        }

        if (!await _moduleService.IsActiveAsync(moduleKey, userId.Value))
        {
            return GateResult.Forbid();
        }

        var handler = _resolver.Resolve(moduleKey);
        if (handler == null)
        {
            return GateResult.NotImplemented($"Module project capability handler was not found for '{moduleKey}'.");
        }

        return GateResult.Allowed(userId.Value, handler);
    }

    private sealed class GateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public Contracts.Services.ModuleCapabilities.IModuleProjectCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static GateResult Allowed(Guid userId, Contracts.Services.ModuleCapabilities.IModuleProjectCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static GateResult Unauthorized() => new() { Result = new UnauthorizedResult() };

        public static GateResult Forbid() => new() { Result = new ForbidResult() };

        public static GateResult NotImplemented(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
