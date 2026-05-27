using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Kits;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/kits")]
public class CraftingKitsController : ControllerBase
{
    private readonly ICraftingKitService _kitService;
    private readonly IModuleService _moduleService;

    public CraftingKitsController(ICraftingKitService kitService, IModuleService moduleService)
    {
        _kitService = kitService;
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CraftingKitDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _kitService.GetAllAsync(userId.Value, includeArchived));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CraftingKitDto>> GetById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var kit = await _kitService.GetByIdAsync(id, userId.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpPost]
    public async Task<ActionResult<CraftingKitDto>> Create(CreateCraftingKitDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var kit = await _kitService.CreateAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = kit.Id }, kit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CraftingKitDto>> Update(Guid id, UpdateCraftingKitDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            request.Id = id;
            var kit = await _kitService.UpdateAsync(request, userId.Value);
            return kit == null ? NotFound() : Ok(kit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<ActionResult<CraftingKitDto>> Archive(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var kit = await _kitService.ArchiveAsync(id, userId.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpPut("{id:guid}/reopen")]
    public async Task<ActionResult<CraftingKitDto>> Reopen(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var kit = await _kitService.ReopenAsync(id, userId.Value);
        return kit == null ? NotFound() : Ok(kit);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _kitService.DeleteAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{kitId:guid}/pieces")]
    public async Task<ActionResult<CraftingKitPieceDto>> AddPiece(Guid kitId, CreateCraftingKitPieceDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var piece = await _kitService.AddPieceAsync(kitId, request, userId.Value);
            return piece == null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = kitId }, piece);
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

    [HttpPut("{kitId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult<CraftingKitPieceDto>> UpdatePiece(
        Guid kitId,
        Guid pieceId,
        UpdateCraftingKitPieceDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            request.Id = pieceId;
            var piece = await _kitService.UpdatePieceAsync(kitId, request, userId.Value);
            return piece == null ? NotFound() : Ok(piece);
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

    [HttpDelete("{kitId:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult> DeletePiece(Guid kitId, Guid pieceId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _kitService.DeletePieceAsync(kitId, pieceId, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{kitId:guid}/pieces/reorder")]
    public async Task<ActionResult> ReorderPieces(Guid kitId, ReorderCraftingKitItemsDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var reordered = await _kitService.ReorderPiecesAsync(kitId, request, userId.Value);
        return reordered ? NoContent() : NotFound();
    }

    [HttpPost("{kitId:guid}/pieces/{pieceId:guid}/project")]
    public async Task<ActionResult<CraftingProjectDto>> CreateProjectForPiece(
        Guid kitId,
        Guid pieceId,
        CreateCraftingKitProjectDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _kitService.CreateProjectForPieceAsync(kitId, pieceId, request, userId.Value);
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

    [HttpPost("{kitId:guid}/supplies")]
    public async Task<ActionResult<CraftingKitSupplyDto>> AddSupply(Guid kitId, CreateCraftingKitSupplyDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var supply = await _kitService.AddSupplyAsync(kitId, request, userId.Value);
            return supply == null ? NotFound() : CreatedAtAction(nameof(GetById), new { id = kitId }, supply);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{kitId:guid}/supplies/{supplyId:guid}")]
    public async Task<ActionResult<CraftingKitSupplyDto>> UpdateSupply(
        Guid kitId,
        Guid supplyId,
        UpdateCraftingKitSupplyDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            request.Id = supplyId;
            var supply = await _kitService.UpdateSupplyAsync(kitId, request, userId.Value);
            return supply == null ? NotFound() : Ok(supply);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{kitId:guid}/supplies/{supplyId:guid}")]
    public async Task<ActionResult> DeleteSupply(Guid kitId, Guid supplyId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _kitService.DeleteSupplyAsync(kitId, supplyId, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{kitId:guid}/supplies/reorder")]
    public async Task<ActionResult> ReorderSupplies(Guid kitId, ReorderCraftingKitItemsDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var reordered = await _kitService.ReorderSuppliesAsync(kitId, request, userId.Value);
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
