using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Server.Data;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/assets")]
public class AssetsController : ControllerBase
{
    private const long MaxUploadBytes = 25 * 1024 * 1024; // 25 MB for initial local-first foundation
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private readonly TankerMadeDbContext _context;
    private readonly IAssetStorageService _assetStorageService;
    private readonly IAssetThumbnailService _assetThumbnailService;
    private readonly IModuleService _moduleService;

    public AssetsController(
        TankerMadeDbContext context,
        IAssetStorageService assetStorageService,
        IAssetThumbnailService assetThumbnailService,
        IModuleService moduleService)
    {
        _context = context;
        _assetStorageService = assetStorageService;
        _assetThumbnailService = assetThumbnailService;
        _moduleService = moduleService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssetRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var asset = await _context.AssetRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value, cancellationToken);

        if (asset == null)
        {
            return NotFound();
        }

        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => t.AssetRecordId == asset.Id)
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(ToDto(asset, thumbnails));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AssetRecordDto>>> List(
        [FromQuery] string moduleKey,
        [FromQuery] string? recordType = null,
        [FromQuery] Guid? recordId = null,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return BadRequest("moduleKey is required.");
        }
        var normalizedModuleKey = moduleKey.Trim();

        var query = _context.AssetRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value && a.ModuleKey == normalizedModuleKey);

        if (!includeDeleted)
        {
            query = query.Where(a => !a.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(recordType))
        {
            query = query.Where(a => a.RecordType == recordType.Trim());
        }

        if (recordId.HasValue)
        {
            query = query.Where(a => a.RecordId == recordId.Value);
        }

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

    [HttpGet("picker")]
    public async Task<ActionResult<IReadOnlyList<AssetRecordDto>>> PickerList(
        [FromQuery] string moduleKey,
        [FromQuery] string recordType,
        [FromQuery] Guid? recordId,
        [FromQuery] bool includeUnassigned = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return BadRequest("moduleKey is required.");
        }

        if (string.IsNullOrWhiteSpace(recordType))
        {
            return BadRequest("recordType is required.");
        }

        var normalizedModuleKey = moduleKey.Trim();
        var normalizedRecordType = recordType.Trim();
        var moduleActive = await _moduleService.IsActiveAsync(normalizedModuleKey, userId.Value);
        if (!moduleActive)
        {
            return Forbid();
        }

        var query = _context.AssetRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value
                && a.ModuleKey == normalizedModuleKey
                && !a.IsDeleted);

        query = includeUnassigned
            ? query.Where(a =>
                (a.RecordType == normalizedRecordType && a.RecordId == recordId)
                || (a.RecordType == string.Empty && a.RecordId == null))
            : query.Where(a => a.RecordType == normalizedRecordType && a.RecordId == recordId);

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

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadBytes)]
    public async Task<ActionResult<AssetRecordDto>> Upload(
        [FromForm] UploadAssetForm request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.ModuleKey))
        {
            return BadRequest("moduleKey is required.");
        }

        if (request.File is null || request.File.Length <= 0)
        {
            return BadRequest("A non-empty file is required.");
        }

        if (request.RecordId.HasValue && string.IsNullOrWhiteSpace(request.RecordType))
        {
            return BadRequest("recordType is required when recordId is provided.");
        }

        if (request.File.Length > MaxUploadBytes)
        {
            return BadRequest($"File exceeds max allowed size ({MaxUploadBytes} bytes).");
        }

        var normalizedModuleKey = request.ModuleKey.Trim();
        var moduleActive = await _moduleService.IsActiveAsync(normalizedModuleKey, userId.Value);
        if (!moduleActive)
        {
            return Forbid();
        }

        var contentType = string.IsNullOrWhiteSpace(request.File.ContentType)
            ? "application/octet-stream"
            : request.File.ContentType.Trim();

        var assetId = Guid.NewGuid();
        var asset = new AssetRecord(
            assetId,
            userId.Value,
            normalizedModuleKey,
            string.IsNullOrWhiteSpace(request.File.FileName) ? "upload.bin" : request.File.FileName,
            contentType,
            request.File.Length,
            storageProvider: _assetStorageService.ProviderName,
            storagePath: "__pending__",
            recordType: string.IsNullOrWhiteSpace(request.RecordType) ? null : request.RecordType,
            recordId: request.RecordId);

        await using var stream = request.File.OpenReadStream();
        var stored = await _assetStorageService.StoreAsync(
            new StoreAssetFileRequest
            {
                AssetId = asset.Id,
                UserId = userId.Value,
                ModuleKey = asset.ModuleKey,
                OriginalFileName = asset.OriginalFileName
            },
            stream,
            cancellationToken);

        asset.StorageProvider = stored.StorageProvider;
        asset.StoragePath = stored.StoragePath;
        asset.FileSizeBytes = stored.FileSizeBytes;
        asset.UpdatedAt = DateTime.UtcNow;

        _context.AssetRecords.Add(asset);
        var thumbnails = await _assetThumbnailService.GenerateAsync(asset, cancellationToken);
        _context.AssetThumbnails.AddRange(thumbnails);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = ToDto(asset, thumbnails);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var asset = await _context.AssetRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value && !a.IsDeleted, cancellationToken);

        if (asset == null)
        {
            return NotFound();
        }

        var stream = await _assetStorageService.OpenReadAsync(asset.StoragePath, cancellationToken);
        if (stream == null)
        {
            return NotFound("Asset file is missing from storage.");
        }

        return File(stream, asset.ContentType, asset.OriginalFileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value, cancellationToken);

        if (asset == null)
        {
            return NotFound();
        }

        if (!asset.IsDeleted)
        {
            await _assetStorageService.DeleteAsync(asset.StoragePath, cancellationToken);
            var thumbnails = await _context.AssetThumbnails
                .Where(t => t.AssetRecordId == asset.Id)
                .ToListAsync(cancellationToken);
            foreach (var thumbnail in thumbnails)
            {
                await _assetThumbnailService.DeleteAsync(thumbnail, cancellationToken);
            }
            asset.MarkDeleted();
            await _context.SaveChangesAsync(cancellationToken);
        }

        return NoContent();
    }

    [HttpGet("maintenance/orphans")]
    public async Task<ActionResult<IReadOnlyList<AssetOrphanAuditItemDto>>> GetOrphans(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var (skip, take) = ResolvePaging(page, pageSize);
        var assets = await _context.AssetRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var assetIds = assets.Select(a => a.Id).ToList();
        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => assetIds.Contains(t.AssetRecordId))
            .ToListAsync(cancellationToken);

        var result = new List<AssetOrphanAuditItemDto>(assets.Count);
        foreach (var asset in assets)
        {
            await using var assetStream = await _assetStorageService.OpenReadAsync(asset.StoragePath, cancellationToken);
            var fileMissing = assetStream == null;

            var assetThumbs = thumbnails.Where(t => t.AssetRecordId == asset.Id).ToList();
            var missingThumbCount = 0;
            foreach (var thumb in assetThumbs)
            {
                await using var thumbStream = await _assetThumbnailService.OpenReadAsync(thumb, cancellationToken);
                if (thumbStream == null)
                {
                    missingThumbCount++;
                }
            }

            result.Add(new AssetOrphanAuditItemDto
            {
                AssetId = asset.Id,
                StoragePath = asset.StoragePath,
                FileMissing = fileMissing,
                HasThumbnailIssues = missingThumbCount > 0,
                MissingThumbnailCount = missingThumbCount
            });
        }

        return Ok(result);
    }

    [HttpPost("maintenance/orphans/mark-deleted")]
    public async Task<ActionResult<IReadOnlyList<Guid>>> MarkOrphansDeleted(
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var assets = await _context.AssetRecords
            .Where(a => a.UserId == userId.Value && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var markedIds = new List<Guid>();
        foreach (var asset in assets)
        {
            await using var stream = await _assetStorageService.OpenReadAsync(asset.StoragePath, cancellationToken);
            if (stream != null)
            {
                continue;
            }

            asset.MarkDeleted();
            markedIds.Add(asset.Id);
        }

        if (markedIds.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Ok(markedIds);
    }

    [HttpPut("{id:guid}/assignment")]
    public async Task<ActionResult<AssetRecordDto>> Assign(
        Guid id,
        AssetAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.ModuleKey))
        {
            return BadRequest("moduleKey is required.");
        }

        if (string.IsNullOrWhiteSpace(request.RecordType))
        {
            return BadRequest("recordType is required.");
        }

        var normalizedModuleKey = request.ModuleKey.Trim();
        var moduleActive = await _moduleService.IsActiveAsync(normalizedModuleKey, userId.Value);
        if (!moduleActive)
        {
            return Forbid();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value && !a.IsDeleted, cancellationToken);
        if (asset == null)
        {
            return NotFound();
        }

        if (!string.Equals(asset.ModuleKey, normalizedModuleKey, StringComparison.Ordinal))
        {
            return BadRequest("Asset moduleKey does not match assignment moduleKey.");
        }

        asset.Reassign(request.RecordType, request.RecordId);
        await _context.SaveChangesAsync(cancellationToken);

        var thumbnails = await _context.AssetThumbnails
            .AsNoTracking()
            .Where(t => t.AssetRecordId == asset.Id)
            .OrderBy(t => t.SizeKey)
            .ToListAsync(cancellationToken);

        return Ok(ToDto(asset, thumbnails));
    }

    [HttpDelete("{id:guid}/assignment")]
    public async Task<ActionResult<AssetRecordDto>> Unassign(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var asset = await _context.AssetRecords
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value && !a.IsDeleted, cancellationToken);
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

    [HttpGet("{id:guid}/thumbnails/{sizeKey}/download")]
    public async Task<ActionResult> DownloadThumbnail(Guid id, string sizeKey, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var asset = await _context.AssetRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value && !a.IsDeleted, cancellationToken);
        if (asset == null)
        {
            return NotFound();
        }

        var normalizedSizeKey = sizeKey.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedSizeKey))
        {
            return BadRequest("sizeKey is required.");
        }

        var thumbnail = await _context.AssetThumbnails
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.AssetRecordId == id && t.SizeKey == normalizedSizeKey, cancellationToken);
        if (thumbnail == null)
        {
            return NotFound();
        }

        var stream = await _assetThumbnailService.OpenReadAsync(thumbnail, cancellationToken);
        if (stream == null)
        {
            return NotFound("Thumbnail file is missing from storage.");
        }

        return File(stream, thumbnail.ContentType);
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

    public sealed class UploadAssetForm
    {
        public string ModuleKey { get; set; } = string.Empty;
        public string? RecordType { get; set; }
        public Guid? RecordId { get; set; }
        public IFormFile? File { get; set; }
    }
}
