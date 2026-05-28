using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Data;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/projects")]
public class CraftingProjectsController : ControllerBase
{
    private const string ProjectRecordType = "project";
    private readonly ICraftingProjectService _projectService;
    private readonly IModuleService _moduleService;
    private readonly TankerMadeDbContext _context;

    public CraftingProjectsController(
        ICraftingProjectService projectService,
        IModuleService moduleService,
        TankerMadeDbContext context)
    {
        _projectService = projectService;
        _moduleService = moduleService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CraftingProjectDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _projectService.GetAllAsync(userId.Value, includeArchived));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CraftingProjectDto>> GetById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.GetByIdAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<CraftingProjectDto>> Create(CreateCraftingProjectDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.CreateAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CraftingProjectDto>> Update(Guid id, UpdateCraftingProjectDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            request.Id = id;
            var project = await _projectService.UpdateAsync(request, userId.Value);
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

    [HttpPut("{id:guid}/archive")]
    public async Task<ActionResult<CraftingProjectDto>> Archive(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.ArchiveAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/reopen")]
    public async Task<ActionResult<CraftingProjectDto>> Reopen(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.ReopenAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/progress")]
    public async Task<ActionResult<CraftingProjectDto>> SetStepProgress(
        Guid id,
        Guid stepId,
        UpdateCraftingProjectStepProgressDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.SetStepProgressAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/start")]
    public async Task<ActionResult<CraftingProjectDto>> StartTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.StartTimerAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/pause")]
    public async Task<ActionResult<CraftingProjectDto>> PauseTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.PauseTimerAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer")]
    public async Task<ActionResult<CraftingProjectDto>> SetTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.SetTimerAsync(id, stepId, request, userId.Value);
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
    public async Task<ActionResult<CraftingProjectDto>> ResetTimer(Guid id, Guid stepId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.ResetTimerAsync(id, stepId, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/inventory-links")]
    public async Task<ActionResult<CraftingProjectDto>> AddInventoryLink(
        Guid id,
        CreateCraftingProjectInventoryLinkDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.AddInventoryLinkAsync(id, request, userId.Value);
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

    [HttpDelete("{id:guid}/inventory-links/{linkId:guid}")]
    public async Task<ActionResult> RemoveInventoryLink(Guid id, Guid linkId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _projectService.RemoveInventoryLinkAsync(id, linkId, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _projectService.DeleteAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/assets")]
    public async Task<ActionResult<IReadOnlyList<AssetRecordDto>>> GetAssets(
        Guid id,
        [FromQuery] bool includeUnassigned = true,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var exists = await _context.CraftingProjects
            .AnyAsync(project => project.Id == id && project.UserId == userId.Value, cancellationToken);
        if (!exists)
        {
            return NotFound();
        }

        var query = _context.AssetRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value
                && a.ModuleKey == CraftingModule.ModuleKey
                && !a.IsDeleted);

        query = includeUnassigned
            ? query.Where(a =>
                (a.RecordType == ProjectRecordType && a.RecordId == id)
                || (a.RecordType == string.Empty && a.RecordId == null))
            : query.Where(a => a.RecordType == ProjectRecordType && a.RecordId == id);

        var assets = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var assetIds = assets.Select(a => a.Id).ToList();
        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => assetIds.Contains(t.AssetRecordId))
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(assets
            .Select(asset => ToDto(asset, thumbnails.Where(t => t.AssetRecordId == asset.Id)))
            .ToList());
    }

    [HttpPut("{id:guid}/assets/{assetId:guid}")]
    public async Task<ActionResult<AssetRecordDto>> AssignAsset(
        Guid id,
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var projectExists = await _context.CraftingProjects
            .AnyAsync(project => project.Id == id && project.UserId == userId.Value, cancellationToken);
        if (!projectExists)
        {
            return NotFound();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == assetId
                && a.UserId == userId.Value
                && a.ModuleKey == CraftingModule.ModuleKey
                && !a.IsDeleted, cancellationToken);
        if (asset == null)
        {
            return NotFound();
        }

        asset.Reassign(ProjectRecordType, id);
        await _context.SaveChangesAsync(cancellationToken);

        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => t.AssetRecordId == asset.Id)
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(ToDto(asset, thumbnails));
    }

    [HttpDelete("{id:guid}/assets/{assetId:guid}")]
    public async Task<ActionResult<AssetRecordDto>> UnassignAsset(
        Guid id,
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var projectExists = await _context.CraftingProjects
            .AnyAsync(project => project.Id == id && project.UserId == userId.Value, cancellationToken);
        if (!projectExists)
        {
            return NotFound();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == assetId
                && a.UserId == userId.Value
                && a.ModuleKey == CraftingModule.ModuleKey
                && !a.IsDeleted
                && a.RecordType == ProjectRecordType
                && a.RecordId == id, cancellationToken);
        if (asset == null)
        {
            return NotFound();
        }

        asset.Reassign(null, null);
        await _context.SaveChangesAsync(cancellationToken);

        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => t.AssetRecordId == asset.Id)
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(ToDto(asset, thumbnails));
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

    private static AssetRecordDto ToDto(AssetRecord asset, IEnumerable<AssetThumbnail> thumbnails)
    {
        return new AssetRecordDto
        {
            Id = asset.Id,
            UserId = asset.UserId,
            ModuleKey = asset.ModuleKey,
            RecordType = asset.RecordType,
            RecordId = asset.RecordId,
            OriginalFileName = asset.OriginalFileName,
            ContentType = asset.ContentType,
            FileSizeBytes = asset.FileSizeBytes,
            StorageProvider = asset.StorageProvider,
            StoragePath = asset.StoragePath,
            IsDeleted = asset.IsDeleted,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
            Thumbnails = thumbnails
                .Select(t => new AssetThumbnailDto
                {
                    Id = t.Id,
                    AssetRecordId = t.AssetRecordId,
                    SizeKey = t.SizeKey,
                    ContentType = t.ContentType,
                    Width = t.Width,
                    Height = t.Height,
                    StorageProvider = t.StorageProvider,
                    StoragePath = t.StoragePath,
                    FileSizeBytes = t.FileSizeBytes,
                    CreatedAt = t.CreatedAt
                })
                .ToList()
        };
    }
}
