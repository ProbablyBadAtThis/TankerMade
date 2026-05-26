using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/patterns")]
public class CraftingPatternsController : ControllerBase
{
    private readonly ICraftingPatternService _patternService;
    private readonly IModuleService _moduleService;

    public CraftingPatternsController(ICraftingPatternService patternService, IModuleService moduleService)
    {
        _patternService = patternService;
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CraftingPatternDto>>> GetAll()
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _patternService.GetAllAsync(userId.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CraftingPatternDto>> GetById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var pattern = await _patternService.GetByIdAsync(id, userId.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpPost]
    public async Task<ActionResult<CraftingPatternDto>> Create(CreateCraftingPatternDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var pattern = await _patternService.CreateAsync(request, userId.Value);
        return CreatedAtAction(nameof(GetById), new { id = pattern.Id }, pattern);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CraftingPatternDto>> Update(Guid id, UpdateCraftingPatternDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        request.Id = id;
        var pattern = await _patternService.UpdateAsync(request, userId.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var deleted = await _patternService.DeleteAsync(id, userId.Value);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{patternId:guid}/pieces")]
    public async Task<ActionResult<CraftingPatternPieceDto>> AddPiece(
        Guid patternId,
        CreateCraftingPatternPieceDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var piece = await _patternService.AddPieceAsync(patternId, request, userId.Value);
        return piece == null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = patternId }, piece);
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult<CraftingPatternPieceDto>> UpdatePiece(
        Guid patternId,
        Guid pieceId,
        UpdateCraftingPatternPieceDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        request.Id = pieceId;
        var piece = await _patternService.UpdatePieceAsync(patternId, request, userId.Value);
        return piece == null ? NotFound() : Ok(piece);
    }

    [HttpDelete("{patternId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult> DeletePiece(Guid patternId, Guid pieceId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var deleted = await _patternService.DeletePieceAsync(patternId, pieceId, userId.Value);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{patternId:guid}/pieces/reorder")]
    public async Task<ActionResult> ReorderPieces(
        Guid patternId,
        ReorderCraftingPatternItemsDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var reordered = await _patternService.ReorderPiecesAsync(patternId, request, userId.Value);
        return reordered ? NoContent() : NotFound();
    }

    [HttpPost("{patternId:guid}/pieces/{pieceId:guid}/steps")]
    public async Task<ActionResult<CraftingPatternStepDto>> AddStep(
        Guid patternId,
        Guid pieceId,
        CreateCraftingPatternStepDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var step = await _patternService.AddStepAsync(patternId, pieceId, request, userId.Value);
        return step == null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = patternId }, step);
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<CraftingPatternStepDto>> UpdateStep(
        Guid patternId,
        Guid pieceId,
        Guid stepId,
        UpdateCraftingPatternStepDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        request.Id = stepId;
        var step = await _patternService.UpdateStepAsync(patternId, pieceId, request, userId.Value);
        return step == null ? NotFound() : Ok(step);
    }

    [HttpDelete("{patternId:guid}/pieces/{pieceId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult> DeleteStep(Guid patternId, Guid pieceId, Guid stepId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var deleted = await _patternService.DeleteStepAsync(patternId, pieceId, stepId, userId.Value);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{patternId:guid}/pieces/{pieceId:guid}/steps/reorder")]
    public async Task<ActionResult> ReorderSteps(
        Guid patternId,
        Guid pieceId,
        ReorderCraftingPatternItemsDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var reordered = await _patternService.ReorderStepsAsync(patternId, pieceId, request, userId.Value);
        return reordered ? NoContent() : NotFound();
    }

    private async Task<Guid?> GetActiveModuleUserIdAsync()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return null;
        }

        return await _moduleService.IsActiveAsync(CraftingModule.ModuleKey, userId.Value)
            ? userId
            : null;
    }
}
