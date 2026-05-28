using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Modules.Printing3D;
using TankerMade.Modules.Printing3D.DTOs.Inventory;
using TankerMade.Modules.Printing3D.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Data;

namespace TankerMade.Server.Controllers.Printing3D;

[ApiController]
[Authorize]
[Route("api/modules/printing-3d/inventory")]
public class PrintingInventoryController : ControllerBase
{
    private const string MaterialRecordType = "material";
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private readonly IPrintingInventoryService _inventoryService;
    private readonly IModuleService _moduleService;
    private readonly TankerMadeDbContext _context;

    public PrintingInventoryController(
        IPrintingInventoryService inventoryService,
        IModuleService moduleService,
        TankerMadeDbContext context)
    {
        _inventoryService = inventoryService;
        _moduleService = moduleService;
        _context = context;
    }

    [HttpGet("materials")]
    public async Task<ActionResult<IReadOnlyList<PrintingMaterialInventoryItemDto>>> GetMaterials(
        [FromQuery] PrintingMaterialInventoryFilterDto filter)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetMaterialsAsync(userId.Value, filter));
    }

    [HttpGet("materials/{id:guid}")]
    public async Task<ActionResult<PrintingMaterialInventoryItemDto>> GetMaterialById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var material = await _inventoryService.GetMaterialByIdAsync(id, userId.Value);
        return material == null ? NotFound() : Ok(material);
    }

    [HttpPost("materials")]
    public async Task<ActionResult<PrintingMaterialInventoryItemDto>> CreateMaterial(
        CreatePrintingMaterialInventoryItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var material = await _inventoryService.CreateOrMergeMaterialAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetMaterialById), new { id = material.Id }, material);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reference/{category}")]
    public async Task<ActionResult<IReadOnlyList<PrintingInventoryReferenceItemDto>>> GetReferenceItems(
        string category)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetReferenceItemsAsync(category));
    }

    [HttpPost("reference/{category}")]
    public async Task<ActionResult<PrintingInventoryReferenceItemDto>> CreateReferenceItem(
        string category,
        CreatePrintingInventoryReferenceItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            return Ok(await _inventoryService.CreateReferenceItemAsync(category, request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("materials/{id:guid}/assets")]
    public async Task<ActionResult<IReadOnlyList<AssetRecordDto>>> GetMaterialAssets(
        Guid id,
        [FromQuery] bool includeUnassigned = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var exists = await _context.PrintingMaterialInventoryItems
            .AnyAsync(item => item.Id == id && item.UserId == userId.Value, cancellationToken);
        if (!exists)
        {
            return NotFound();
        }

        var query = _context.AssetRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value
                && a.ModuleKey == Printing3DModule.ModuleKey
                && !a.IsDeleted);

        query = includeUnassigned
            ? query.Where(a =>
                (a.RecordType == MaterialRecordType && a.RecordId == id)
                || (a.RecordType == string.Empty && a.RecordId == null))
            : query.Where(a => a.RecordType == MaterialRecordType && a.RecordId == id);

        var (skip, take) = ResolvePaging(page, pageSize);
        var assets = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
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

    [HttpPut("materials/{id:guid}/assets/{assetId:guid}")]
    public async Task<ActionResult<AssetRecordDto>> AssignMaterialAsset(
        Guid id,
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var materialExists = await _context.PrintingMaterialInventoryItems
            .AnyAsync(item => item.Id == id && item.UserId == userId.Value, cancellationToken);
        if (!materialExists)
        {
            return NotFound();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == assetId
                && a.UserId == userId.Value
                && a.ModuleKey == Printing3DModule.ModuleKey
                && !a.IsDeleted, cancellationToken);
        if (asset == null)
        {
            return NotFound();
        }

        asset.Reassign(MaterialRecordType, id);
        await _context.SaveChangesAsync(cancellationToken);

        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => t.AssetRecordId == asset.Id)
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(ToDto(asset, thumbnails));
    }

    [HttpDelete("materials/{id:guid}/assets/{assetId:guid}")]
    public async Task<ActionResult<AssetRecordDto>> UnassignMaterialAsset(
        Guid id,
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var materialExists = await _context.PrintingMaterialInventoryItems
            .AnyAsync(item => item.Id == id && item.UserId == userId.Value, cancellationToken);
        if (!materialExists)
        {
            return NotFound();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == assetId
                && a.UserId == userId.Value
                && a.ModuleKey == Printing3DModule.ModuleKey
                && !a.IsDeleted
                && a.RecordType == MaterialRecordType
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

        return await _moduleService.IsActiveAsync(Printing3DModule.ModuleKey, userId.Value)
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

    private static (int Skip, int Take) ResolvePaging(int page, int pageSize)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return ((safePage - 1) * safePageSize, safePageSize);
    }
}
