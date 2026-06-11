using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModuleKits;
using TankerMade.Contracts.DTOs.ModuleProjects;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Controllers.ModuleCapabilities;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/kits")]
public class ModuleKitsController : ControllerBase
{
    private readonly IModuleService _moduleService;
    private readonly IModuleKitCapabilityResolver _resolver;

    public ModuleKitsController(IModuleService moduleService, IModuleKitCapabilityResolver resolver)
    {
        _moduleService = moduleService;
        _resolver = resolver;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModuleKitDto>>> GetAll(string moduleKey, [FromQuery] bool includeArchived = false)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return Ok(await gate.Handler!.GetAllAsync(gate.UserId!.Value, includeArchived));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModuleKitDto>> GetById(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var kit = await gate.Handler!.GetByIdAsync(id, gate.UserId!.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpPost]
    public async Task<ActionResult<ModuleKitDto>> Create(string moduleKey, CreateModuleKitRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var kit = await gate.Handler!.CreateAsync(request, gate.UserId!.Value);
        return Ok(kit);
    }

    [HttpPut("{kitId:guid}")]
    public async Task<ActionResult<ModuleKitDto>> Update(string moduleKey, Guid kitId, UpdateModuleKitRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var kit = await gate.Handler!.UpdateAsync(kitId, request, gate.UserId!.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpDelete("{kitId:guid}")]
    public async Task<ActionResult> Delete(string moduleKey, Guid kitId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return await gate.Handler!.DeleteAsync(kitId, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPut("{kitId:guid}/archive")]
    public async Task<ActionResult<ModuleKitDto>> Archive(string moduleKey, Guid kitId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var kit = await gate.Handler!.ArchiveAsync(kitId, gate.UserId!.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpPut("{kitId:guid}/reopen")]
    public async Task<ActionResult<ModuleKitDto>> Reopen(string moduleKey, Guid kitId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var kit = await gate.Handler!.ReopenAsync(kitId, gate.UserId!.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpPost("{kitId:guid}/pieces")]
    public async Task<ActionResult<ModuleKitPieceDto>> AddPiece(string moduleKey, Guid kitId, CreateModuleKitPieceRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var piece = await gate.Handler!.AddPieceAsync(kitId, request, gate.UserId!.Value);
        return piece == null ? NotFound() : Ok(piece);
    }

    [HttpPut("{kitId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult<ModuleKitPieceDto>> UpdatePiece(
        string moduleKey,
        Guid kitId,
        Guid pieceId,
        UpdateModuleKitPieceRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var piece = await gate.Handler!.UpdatePieceAsync(kitId, pieceId, request, gate.UserId!.Value);
        return piece == null ? NotFound() : Ok(piece);
    }

    [HttpDelete("{kitId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult> DeletePiece(string moduleKey, Guid kitId, Guid pieceId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return await gate.Handler!.DeletePieceAsync(kitId, pieceId, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPost("{kitId:guid}/supplies")]
    public async Task<ActionResult<ModuleKitSupplyDto>> AddSupply(string moduleKey, Guid kitId, CreateModuleKitSupplyRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var supply = await gate.Handler!.AddSupplyAsync(kitId, request, gate.UserId!.Value);
        return supply == null ? NotFound() : Ok(supply);
    }

    [HttpPut("{kitId:guid}/supplies/{supplyId:guid}")]
    public async Task<ActionResult<ModuleKitSupplyDto>> UpdateSupply(
        string moduleKey,
        Guid kitId,
        Guid supplyId,
        UpdateModuleKitSupplyRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var supply = await gate.Handler!.UpdateSupplyAsync(kitId, supplyId, request, gate.UserId!.Value);
        return supply == null ? NotFound() : Ok(supply);
    }

    [HttpDelete("{kitId:guid}/supplies/{supplyId:guid}")]
    public async Task<ActionResult> DeleteSupply(string moduleKey, Guid kitId, Guid supplyId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return await gate.Handler!.DeleteSupplyAsync(kitId, supplyId, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPost("{kitId:guid}/pieces/{pieceId:guid}/project")]
    public async Task<ActionResult<ModuleProjectDto>> CreateProjectForPiece(string moduleKey, Guid kitId, Guid pieceId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        var project = await gate.Handler!.CreateProjectForPieceAsync(kitId, pieceId, gate.UserId!.Value);
        return project == null ? NotFound() : Ok(project);
    }

    private async Task<GateResult> ResolveGateAsync(string moduleKey)
    {
        var userId = User.GetUserId();
        if (userId == null) return GateResult.Unauthorized();
        if (!await _moduleService.IsActiveAsync(moduleKey, userId.Value)) return GateResult.Forbid();

        var handler = _resolver.Resolve(moduleKey);
        if (handler == null) return GateResult.NotImplemented($"Module kit capability handler was not found for '{moduleKey}'.");

        return GateResult.Allowed(userId.Value, handler);
    }

    private sealed class GateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public Contracts.Services.ModuleCapabilities.IModuleKitCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static GateResult Allowed(Guid userId, Contracts.Services.ModuleCapabilities.IModuleKitCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static GateResult Unauthorized() => new() { Result = new UnauthorizedResult() };
        public static GateResult Forbid() => new() { Result = new ForbidResult() };
        public static GateResult NotImplemented(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
