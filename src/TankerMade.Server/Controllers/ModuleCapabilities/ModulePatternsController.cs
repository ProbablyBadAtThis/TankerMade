using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModulePatterns;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Controllers.ModuleCapabilities;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/patterns")]
public class ModulePatternsController : ControllerBase
{
    private const int DefaultPageSize = 50;
    private readonly IModuleService _moduleService;
    private readonly IModulePatternCapabilityResolver _resolver;

    public ModulePatternsController(IModuleService moduleService, IModulePatternCapabilityResolver resolver)
    {
        _moduleService = moduleService;
        _resolver = resolver;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModulePatternDto>>> GetAll(
        string moduleKey,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetAllAsync(gate.UserId!.Value, page, pageSize));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ModulePatternDto>>> Search(
        string moduleKey,
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(Array.Empty<ModulePatternDto>());
        }

        return Ok(await gate.Handler!.SearchAsync(q, gate.UserId!.Value, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModulePatternDto>> GetById(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var pattern = await gate.Handler!.GetByIdAsync(id, gate.UserId!.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpPost]
    public async Task<ActionResult<ModulePatternDto>> Create(string moduleKey, CreateModulePatternRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var pattern = await gate.Handler!.CreateAsync(request, gate.UserId!.Value);
        return CreatedAtAction(nameof(GetById), new { moduleKey, id = pattern.Id }, pattern);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ModulePatternDto>> Update(string moduleKey, Guid id, UpdateModulePatternRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        request.Id = id;
        var pattern = await gate.Handler!.UpdateAsync(request, gate.UserId!.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(string moduleKey, Guid id)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            return await gate.Handler!.DeleteAsync(id, gate.UserId!.Value)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{patternId:guid}/pieces")]
    public async Task<ActionResult<ModulePatternPieceDto>> AddPiece(string moduleKey, Guid patternId, CreateModulePatternPieceRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var piece = await gate.Handler!.AddPieceAsync(patternId, request, gate.UserId!.Value);
        return piece == null ? NotFound() : CreatedAtAction(nameof(GetById), new { moduleKey, id = patternId }, piece);
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult<ModulePatternPieceDto>> UpdatePiece(
        string moduleKey,
        Guid patternId,
        Guid pieceId,
        UpdateModulePatternPieceRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        request.Id = pieceId;
        var piece = await gate.Handler!.UpdatePieceAsync(patternId, request, gate.UserId!.Value);
        return piece == null ? NotFound() : Ok(piece);
    }

    [HttpDelete("{patternId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult> DeletePiece(string moduleKey, Guid patternId, Guid pieceId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            return await gate.Handler!.DeletePieceAsync(patternId, pieceId, gate.UserId!.Value)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{patternId:guid}/pieces/reorder")]
    public async Task<ActionResult> ReorderPieces(string moduleKey, Guid patternId, ReorderModulePatternItemsRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.ReorderPiecesAsync(patternId, request, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPost("{patternId:guid}/pieces/{pieceId:guid}/steps")]
    public async Task<ActionResult<ModulePatternStepDto>> AddStep(
        string moduleKey,
        Guid patternId,
        Guid pieceId,
        CreateModulePatternStepRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var step = await gate.Handler!.AddStepAsync(patternId, pieceId, request, gate.UserId!.Value);
        return step == null ? NotFound() : CreatedAtAction(nameof(GetById), new { moduleKey, id = patternId }, step);
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ModulePatternStepDto>> UpdateStep(
        string moduleKey,
        Guid patternId,
        Guid pieceId,
        Guid stepId,
        UpdateModulePatternStepRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        request.Id = stepId;
        var step = await gate.Handler!.UpdateStepAsync(patternId, pieceId, request, gate.UserId!.Value);
        return step == null ? NotFound() : Ok(step);
    }

    [HttpDelete("{patternId:guid}/pieces/{pieceId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult> DeleteStep(string moduleKey, Guid patternId, Guid pieceId, Guid stepId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        try
        {
            return await gate.Handler!.DeleteStepAsync(patternId, pieceId, stepId, gate.UserId!.Value)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}/steps/reorder")]
    public async Task<ActionResult> ReorderSteps(
        string moduleKey,
        Guid patternId,
        Guid pieceId,
        ReorderModulePatternItemsRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.ReorderStepsAsync(patternId, pieceId, request, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    [HttpPost("{patternId:guid}/supplies")]
    public async Task<ActionResult<ModulePatternSupplyDto>> AddSupply(
        string moduleKey,
        Guid patternId,
        CreateModulePatternSupplyRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var supply = await gate.Handler!.AddSupplyAsync(patternId, request, gate.UserId!.Value);
        return supply == null ? NotFound() : CreatedAtAction(nameof(GetById), new { moduleKey, id = patternId }, supply);
    }

    [HttpDelete("{patternId:guid}/supplies/{supplyId:guid}")]
    public async Task<ActionResult> DeleteSupply(string moduleKey, Guid patternId, Guid supplyId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.DeleteSupplyAsync(patternId, supplyId, gate.UserId!.Value)
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
            return GateResult.NotFound($"Module capability handler was not found for '{moduleKey}'.");
        }

        return GateResult.Allowed(userId.Value, handler);
    }

    private sealed class GateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public Contracts.Services.ModuleCapabilities.IModulePatternCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static GateResult Allowed(Guid userId, Contracts.Services.ModuleCapabilities.IModulePatternCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static GateResult Unauthorized() => new() { Result = new UnauthorizedResult() };

        public static GateResult Forbid() => new() { Result = new ForbidResult() };

        public static GateResult NotFound(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
