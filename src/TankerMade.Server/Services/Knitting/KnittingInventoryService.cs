using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TankerMade.Modules.Knitting.DTOs.Inventory;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.ReferenceData;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;
using TankerMade.Server.Services.ReferenceData;

namespace TankerMade.Server.Services.Knitting;

public class KnittingInventoryService : IKnittingInventoryService
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private static readonly TimeSpan CoreReferenceCacheTtl = TimeSpan.FromMinutes(10);
    private readonly TankerMadeDbContext _context;
    private readonly IMemoryCache _cache;

    public KnittingInventoryService(TankerMadeDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public KnittingInventoryService(TankerMadeDbContext context)
        : this(context, new MemoryCache(new MemoryCacheOptions()))
    {
    }

    public async Task<KnittingYarnInventoryItemDto> CreateOrMergeYarnAsync(
        CreateKnittingYarnInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateYarn(createDto);

        var normalizedBrand = KnittingYarnInventoryItem.Normalize(createDto.BrandName);
        var normalizedColor = KnittingYarnInventoryItem.Normalize(createDto.ColorName);

        var yarn = await _context.KnittingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.UserId == userId
                && y.NormalizedBrandName == normalizedBrand
                && y.NormalizedColorName == normalizedColor);

        if (yarn == null)
        {
            yarn = new KnittingYarnInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.ColorName);

            _context.KnittingYarnInventoryItems.Add(yarn);
        }

        yarn.MergeDetails(
            createDto.MainColor,
            createDto.WeightName,
            createDto.FiberContent,
            createDto.FiberTag,
            createDto.Skeins,
            createDto.EstimatedLength,
            createDto.LengthUnit,
            createDto.CurrentWeight,
            createDto.Price,
            createDto.IsSalePrice);

        await MergeLotAsync(yarn, createDto);
        AddPurchase(yarn, createDto);

        await _context.SaveChangesAsync();
        return await MapYarnAsync(yarn.Id) ?? throw new InvalidOperationException("Created yarn could not be loaded.");
    }

    public async Task<KnittingYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId)
    {
        var yarn = await _context.KnittingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == id && y.UserId == userId);

        return yarn == null ? null : await MapYarnAsync(yarn.Id);
    }

    public async Task<KnittingYarnInventoryItemDto?> UpdateYarnRemainingAsync(
        Guid id,
        UpdateKnittingYarnRemainingDto request,
        Guid userId)
    {
        var yarn = await _context.KnittingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == id && y.UserId == userId);
        if (yarn == null)
        {
            return null;
        }

        yarn.SetRemainingMeasurements(
            request.EstimatedRemainingLength,
            request.LengthUnit,
            request.CurrentWeight);
        await _context.SaveChangesAsync();
        return await MapYarnAsync(yarn.Id);
    }

    public async Task<KnittingYarnInventoryItemDto?> UpdateYarnLotRemainingAsync(
        Guid yarnId,
        Guid lotId,
        UpdateKnittingYarnLotRemainingDto request,
        Guid userId)
    {
        var yarn = await _context.KnittingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == yarnId && y.UserId == userId);
        if (yarn == null)
        {
            return null;
        }

        var lot = await _context.KnittingYarnLots
            .SingleOrDefaultAsync(l => l.Id == lotId && l.YarnInventoryItemId == yarnId);
        if (lot == null)
        {
            return null;
        }

        lot.SetRemaining(request.RemainingLength, request.CurrentWeight);
        await _context.SaveChangesAsync();
        return await MapYarnAsync(yarnId);
    }

    public async Task<IReadOnlyList<KnittingYarnInventoryItemDto>> GetYarnsAsync(
        Guid userId,
        KnittingYarnInventoryFilterDto? filter = null)
    {
        var query = ApplyYarnFilters(
            _context.KnittingYarnInventoryItems.Where(y => y.UserId == userId),
            filter);
        var (skip, take) = ResolvePaging(filter?.Page ?? 1, filter?.PageSize ?? DefaultPageSize);

        var yarns = await query
            .OrderBy(y => y.BrandName)
            .ThenBy(y => y.ColorName)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var result = new List<KnittingYarnInventoryItemDto>();
        foreach (var yarn in yarns)
        {
            var dto = await MapYarnAsync(yarn.Id);
            if (dto != null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    public async Task<KnittingToolInventoryItemDto> CreateOrMergeToolAsync(
        CreateKnittingToolInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateTool(createDto);

        var normalizedBrand = KnittingToolInventoryItem.Normalize(createDto.BrandName);
        var normalizedType = KnittingToolInventoryItem.Normalize(createDto.TypeName);

        var tool = await _context.KnittingToolInventoryItems
            .SingleOrDefaultAsync(t => t.UserId == userId
                && t.NormalizedBrandName == normalizedBrand
                && t.NormalizedTypeName == normalizedType);

        if (tool == null)
        {
            tool = new KnittingToolInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.TypeName);

            _context.KnittingToolInventoryItems.Add(tool);
        }

        tool.MergeDetails(
            createDto.Size,
            createDto.Description,
            createDto.Quantity,
            createDto.Price,
            createDto.IsSalePrice);

        AddToolPurchase(tool, createDto);

        await _context.SaveChangesAsync();
        return await MapToolAsync(tool.Id) ?? throw new InvalidOperationException("Created tool could not be loaded.");
    }

    public async Task<KnittingToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId)
    {
        var tool = await _context.KnittingToolInventoryItems
            .SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        return tool == null ? null : await MapToolAsync(tool.Id);
    }

    public async Task<IReadOnlyList<KnittingToolInventoryItemDto>> GetToolsAsync(
        Guid userId,
        KnittingToolInventoryFilterDto? filter = null)
    {
        var query = ApplyToolFilters(
            _context.KnittingToolInventoryItems.Where(t => t.UserId == userId),
            filter);
        var (skip, take) = ResolvePaging(filter?.Page ?? 1, filter?.PageSize ?? DefaultPageSize);

        var tools = await query
            .OrderBy(t => t.TypeName)
            .ThenBy(t => t.BrandName)
            .ThenBy(t => t.Size)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var result = new List<KnittingToolInventoryItemDto>();
        foreach (var tool in tools)
        {
            var dto = await MapToolAsync(tool.Id);
            if (dto != null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    public async Task<KnittingNotionInventoryItemDto> CreateOrMergeNotionAsync(
        CreateKnittingNotionInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateNotion(createDto);

        var normalizedBrand = KnittingNotionInventoryItem.Normalize(createDto.BrandName);
        var normalizedType = KnittingNotionInventoryItem.Normalize(createDto.TypeName);

        var notion = await _context.KnittingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.UserId == userId
                && n.NormalizedBrandName == normalizedBrand
                && n.NormalizedTypeName == normalizedType);

        if (notion == null)
        {
            notion = new KnittingNotionInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.TypeName);

            _context.KnittingNotionInventoryItems.Add(notion);
        }

        notion.MergeDetails(
            createDto.Size,
            createDto.ColorName,
            createDto.Description,
            createDto.Quantity,
            createDto.Price,
            createDto.IsSalePrice);

        AddNotionPurchase(notion, createDto);

        await _context.SaveChangesAsync();
        return await MapNotionAsync(notion.Id) ?? throw new InvalidOperationException("Created notion could not be loaded.");
    }

    public async Task<KnittingNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId)
    {
        var notion = await _context.KnittingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        return notion == null ? null : await MapNotionAsync(notion.Id);
    }

    public async Task<IReadOnlyList<KnittingNotionInventoryItemDto>> GetNotionsAsync(
        Guid userId,
        KnittingNotionInventoryFilterDto? filter = null)
    {
        var query = ApplyNotionFilters(
            _context.KnittingNotionInventoryItems.Where(n => n.UserId == userId),
            filter);
        var (skip, take) = ResolvePaging(filter?.Page ?? 1, filter?.PageSize ?? DefaultPageSize);

        var notions = await query
            .OrderBy(n => n.TypeName)
            .ThenBy(n => n.BrandName)
            .ThenBy(n => n.Size)
            .ThenBy(n => n.ColorName)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var result = new List<KnittingNotionInventoryItemDto>();
        foreach (var notion in notions)
        {
            var dto = await MapNotionAsync(notion.Id);
            if (dto != null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    public async Task<IReadOnlyList<KnittingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category)
    {
        var coreReferenceItems = await CoreReferenceLookup.TryGetItemsAsync(_context, category);
        if (coreReferenceItems != null)
        {
            var cacheKey = $"core-reference:crafting:{category.Trim().ToLowerInvariant()}";
            if (_cache.TryGetValue<IReadOnlyList<KnittingInventoryReferenceItemDto>>(cacheKey, out var cached)
                && cached != null)
            {
                return cached;
            }

            var result = coreReferenceItems
                    .Select((item, index) => new KnittingInventoryReferenceItemDto
                    {
                        Id = item.Id,
                        Category = item.Category,
                        Name = item.Name,
                        Slug = item.Slug,
                        SortOrder = index + 1
                    })
                    .ToList();

            _cache.Set(cacheKey, result, CoreReferenceCacheTtl);
            return result;
        }

        if (!KnittingReferenceCategories.IsModuleOwned(category))
        {
            return [];
        }

        var normalizedCategory = category.Trim();

        return await _context.KnittingInventoryReferenceItems
            .Where(i => i.Category == normalizedCategory)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Name)
            .Select(i => new KnittingInventoryReferenceItemDto
            {
                Id = i.Id,
                Category = i.Category,
                Name = i.Name,
                Slug = i.Slug,
                SortOrder = i.SortOrder
            })
            .ToListAsync();
    }

    public async Task<KnittingInventoryReferenceItemDto> CreateReferenceItemAsync(
        string category,
        CreateKnittingInventoryReferenceItemDto request)
    {
        if (!KnittingReferenceCategories.IsModuleOwned(category))
        {
            throw new ArgumentException("This category is not module-owned.", nameof(category));
        }

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Reference item name is required.", nameof(request));
        }

        var normalizedCategory = category.Trim();
        var slug = ToSlug(name);
        var existing = await _context.KnittingInventoryReferenceItems
            .SingleOrDefaultAsync(i => i.Category == normalizedCategory && i.Slug == slug);

        if (existing != null)
        {
            return new KnittingInventoryReferenceItemDto
            {
                Id = existing.Id,
                Category = existing.Category,
                Name = existing.Name,
                Slug = existing.Slug,
                SortOrder = existing.SortOrder
            };
        }

        var maxSortOrder = await _context.KnittingInventoryReferenceItems
            .Where(i => i.Category == normalizedCategory)
            .Select(i => (int?)i.SortOrder)
            .MaxAsync() ?? 0;

        var created = new KnittingInventoryReferenceItem(
            Guid.NewGuid(),
            normalizedCategory,
            name,
            maxSortOrder + 1);

        _context.KnittingInventoryReferenceItems.Add(created);
        await _context.SaveChangesAsync();

        return new KnittingInventoryReferenceItemDto
        {
            Id = created.Id,
            Category = created.Category,
            Name = created.Name,
            Slug = created.Slug,
            SortOrder = created.SortOrder
        };
    }

    private IQueryable<KnittingYarnInventoryItem> ApplyYarnFilters(
        IQueryable<KnittingYarnInventoryItem> query,
        KnittingYarnInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = KnittingYarnInventoryItem.Normalize(filter.BrandName);
            query = query.Where(y => y.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.ColorName))
        {
            var normalized = KnittingYarnInventoryItem.Normalize(filter.ColorName);
            query = query.Where(y => y.NormalizedColorName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.MainColor))
        {
            var value = filter.MainColor.Trim();
            query = query.Where(y => y.MainColor.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.WeightName))
        {
            var value = filter.WeightName.Trim();
            query = query.Where(y => y.WeightName.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.FiberTag))
        {
            var value = filter.FiberTag.Trim();
            query = query.Where(y => y.FiberTag.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.SourceName))
        {
            var value = filter.SourceName.Trim();
            query = query.Where(y => _context.KnittingYarnPurchases
                .Any(p => p.YarnInventoryItemId == y.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = KnittingYarnInventoryItem.Normalize(value);
            query = query.Where(y => y.NormalizedBrandName.Contains(normalized)
                || y.NormalizedColorName.Contains(normalized)
                || y.MainColor.Contains(value)
                || y.WeightName.Contains(value)
                || y.FiberContent.Contains(value)
                || y.FiberTag.Contains(value));
        }

        return query;
    }

    private IQueryable<KnittingToolInventoryItem> ApplyToolFilters(
        IQueryable<KnittingToolInventoryItem> query,
        KnittingToolInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = KnittingToolInventoryItem.Normalize(filter.BrandName);
            query = query.Where(t => t.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.TypeName))
        {
            var normalized = KnittingToolInventoryItem.Normalize(filter.TypeName);
            query = query.Where(t => t.NormalizedTypeName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.Size))
        {
            var value = filter.Size.Trim();
            query = query.Where(t => t.Size.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.SourceName))
        {
            var value = filter.SourceName.Trim();
            query = query.Where(t => _context.KnittingToolPurchases
                .Any(p => p.ToolInventoryItemId == t.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = KnittingToolInventoryItem.Normalize(value);
            query = query.Where(t => t.NormalizedBrandName.Contains(normalized)
                || t.NormalizedTypeName.Contains(normalized)
                || t.Size.Contains(value)
                || t.Description.Contains(value));
        }

        return query;
    }

    private IQueryable<KnittingNotionInventoryItem> ApplyNotionFilters(
        IQueryable<KnittingNotionInventoryItem> query,
        KnittingNotionInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = KnittingNotionInventoryItem.Normalize(filter.BrandName);
            query = query.Where(n => n.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.TypeName))
        {
            var normalized = KnittingNotionInventoryItem.Normalize(filter.TypeName);
            query = query.Where(n => n.NormalizedTypeName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.Size))
        {
            var value = filter.Size.Trim();
            query = query.Where(n => n.Size.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.ColorName))
        {
            var value = filter.ColorName.Trim();
            query = query.Where(n => n.ColorName.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.SourceName))
        {
            var value = filter.SourceName.Trim();
            query = query.Where(n => _context.KnittingNotionPurchases
                .Any(p => p.NotionInventoryItemId == n.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = KnittingNotionInventoryItem.Normalize(value);
            query = query.Where(n => n.NormalizedBrandName.Contains(normalized)
                || n.NormalizedTypeName.Contains(normalized)
                || n.Size.Contains(value)
                || n.ColorName.Contains(value)
                || n.Description.Contains(value));
        }

        return query;
    }

    private async Task MergeLotAsync(KnittingYarnInventoryItem yarn, CreateKnittingYarnInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.LotNumber))
        {
            return;
        }

        var lotNumber = createDto.LotNumber.Trim();
        var lot = await _context.KnittingYarnLots
            .SingleOrDefaultAsync(l => l.YarnInventoryItemId == yarn.Id && l.LotNumber == lotNumber);

        if (lot == null)
        {
            _context.KnittingYarnLots.Add(new KnittingYarnLot(
                Guid.NewGuid(),
                yarn.Id,
                lotNumber,
                createDto.Skeins,
                createDto.EstimatedLength,
                createDto.CurrentWeight));
            return;
        }

        lot.Merge(createDto.Skeins, createDto.EstimatedLength, createDto.CurrentWeight);
    }

    private void AddPurchase(KnittingYarnInventoryItem yarn, CreateKnittingYarnInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.KnittingYarnPurchases.Add(new KnittingYarnPurchase(
            Guid.NewGuid(),
            yarn.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private void AddToolPurchase(KnittingToolInventoryItem tool, CreateKnittingToolInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.KnittingToolPurchases.Add(new KnittingToolPurchase(
            Guid.NewGuid(),
            tool.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private void AddNotionPurchase(KnittingNotionInventoryItem notion, CreateKnittingNotionInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.KnittingNotionPurchases.Add(new KnittingNotionPurchase(
            Guid.NewGuid(),
            notion.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private static void ValidateCreateYarn(CreateKnittingYarnInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.BrandName))
        {
            throw new ArgumentException("Brand name is required.", nameof(createDto));
        }

        if (string.IsNullOrWhiteSpace(createDto.ColorName))
        {
            throw new ArgumentException("Color name is required.", nameof(createDto));
        }

        if (createDto.Skeins <= 0)
        {
            throw new ArgumentException("Skeins must be greater than zero.", nameof(createDto));
        }
    }

    private static void ValidateCreateTool(CreateKnittingToolInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.BrandName))
        {
            throw new ArgumentException("Brand name is required.", nameof(createDto));
        }

        if (string.IsNullOrWhiteSpace(createDto.TypeName))
        {
            throw new ArgumentException("Tool type is required.", nameof(createDto));
        }

        if (createDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(createDto));
        }
    }

    private static void ValidateCreateNotion(CreateKnittingNotionInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.BrandName))
        {
            throw new ArgumentException("Brand name is required.", nameof(createDto));
        }

        if (string.IsNullOrWhiteSpace(createDto.TypeName))
        {
            throw new ArgumentException("Notion type is required.", nameof(createDto));
        }

        if (createDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(createDto));
        }
    }

    private static string ToSlug(string value)
    {
        return value.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", string.Empty);
    }

    private async Task<KnittingYarnInventoryItemDto?> MapYarnAsync(Guid id)
    {
        var yarn = await _context.KnittingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == id);

        if (yarn == null)
        {
            return null;
        }

        var lots = await _context.KnittingYarnLots
            .Where(l => l.YarnInventoryItemId == id)
            .OrderBy(l => l.LotNumber)
            .Select(l => new KnittingYarnLotDto
            {
                Id = l.Id,
                YarnInventoryItemId = l.YarnInventoryItemId,
                LotNumber = l.LotNumber,
                Skeins = l.Skeins,
                RemainingLength = l.RemainingLength,
                CurrentWeight = l.CurrentWeight,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            })
            .ToListAsync();

        var purchases = await _context.KnittingYarnPurchases
            .Where(p => p.YarnInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new KnittingYarnPurchaseDto
            {
                Id = p.Id,
                YarnInventoryItemId = p.YarnInventoryItemId,
                SourceName = p.SourceName,
                Price = p.Price,
                IsSalePrice = p.IsSalePrice,
                PurchasedAt = p.PurchasedAt,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new KnittingYarnInventoryItemDto
        {
            Id = yarn.Id,
            UserId = yarn.UserId,
            BrandName = yarn.BrandName,
            ColorName = yarn.ColorName,
            MainColor = yarn.MainColor,
            WeightName = yarn.WeightName,
            FiberContent = yarn.FiberContent,
            FiberTag = yarn.FiberTag,
            TotalSkeins = yarn.TotalSkeins,
            EstimatedRemainingLength = yarn.EstimatedRemainingLength,
            LengthUnit = yarn.LengthUnit,
            CurrentWeight = yarn.CurrentWeight,
            RegularPrice = yarn.RegularPrice,
            Lots = lots,
            Purchases = purchases,
            CreatedAt = yarn.CreatedAt,
            UpdatedAt = yarn.UpdatedAt
        };
    }

    private async Task<KnittingToolInventoryItemDto?> MapToolAsync(Guid id)
    {
        var tool = await _context.KnittingToolInventoryItems
            .SingleOrDefaultAsync(t => t.Id == id);

        if (tool == null)
        {
            return null;
        }

        var purchases = await _context.KnittingToolPurchases
            .Where(p => p.ToolInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new KnittingToolPurchaseDto
            {
                Id = p.Id,
                ToolInventoryItemId = p.ToolInventoryItemId,
                SourceName = p.SourceName,
                Price = p.Price,
                IsSalePrice = p.IsSalePrice,
                PurchasedAt = p.PurchasedAt,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new KnittingToolInventoryItemDto
        {
            Id = tool.Id,
            UserId = tool.UserId,
            BrandName = tool.BrandName,
            TypeName = tool.TypeName,
            Size = tool.Size,
            Description = tool.Description,
            Quantity = tool.Quantity,
            RegularPrice = tool.RegularPrice,
            Purchases = purchases,
            CreatedAt = tool.CreatedAt,
            UpdatedAt = tool.UpdatedAt
        };
    }

    private async Task<KnittingNotionInventoryItemDto?> MapNotionAsync(Guid id)
    {
        var notion = await _context.KnittingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.Id == id);

        if (notion == null)
        {
            return null;
        }

        var purchases = await _context.KnittingNotionPurchases
            .Where(p => p.NotionInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new KnittingNotionPurchaseDto
            {
                Id = p.Id,
                NotionInventoryItemId = p.NotionInventoryItemId,
                SourceName = p.SourceName,
                Price = p.Price,
                IsSalePrice = p.IsSalePrice,
                PurchasedAt = p.PurchasedAt,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new KnittingNotionInventoryItemDto
        {
            Id = notion.Id,
            UserId = notion.UserId,
            BrandName = notion.BrandName,
            TypeName = notion.TypeName,
            Size = notion.Size,
            ColorName = notion.ColorName,
            Description = notion.Description,
            Quantity = notion.Quantity,
            RegularPrice = notion.RegularPrice,
            Purchases = purchases,
            CreatedAt = notion.CreatedAt,
            UpdatedAt = notion.UpdatedAt
        };
    }

    private static (int Skip, int Take) ResolvePaging(int page, int pageSize)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return ((safePage - 1) * safePageSize, safePageSize);
    }

    public async Task<IReadOnlyList<KnittingSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null)
    {
        var query = _context.KnittingSupplyItems.Where(item => item.UserId == userId);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim().ToLowerInvariant();
            query = query.Where(item => item.Category.ToLower() == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            var like = $"%{term}%";
            query = query.Where(item =>
                EF.Functions.Like((item.Name ?? string.Empty).ToLower(), like)
                || EF.Functions.Like((item.Brand ?? string.Empty).ToLower(), like)
                || EF.Functions.Like((item.Color ?? string.Empty).ToLower(), like)
                || EF.Functions.Like((item.Notes ?? string.Empty).ToLower(), like));
        }

        var items = await query
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Name)
            .ToListAsync();

        return items.Select(MapLegacySupply).ToList();
    }

    public async Task<KnittingSupplyItemDto> CreateSupplyAsync(CreateKnittingSupplyItemDto createDto, Guid userId)
    {
        var entity = new KnittingSupplyItem(Guid.NewGuid(), userId, createDto.Category, createDto.Name);
        entity.Update(createDto.Brand, createDto.Color, createDto.Size, createDto.Quantity, createDto.Unit, createDto.Notes);

        _context.KnittingSupplyItems.Add(entity);
        await _context.SaveChangesAsync();

        return MapLegacySupply(entity);
    }

    public async Task<KnittingSupplyItemDto?> UpdateSupplyAsync(Guid supplyId, CreateKnittingSupplyItemDto updateDto, Guid userId)
    {
        var entity = await _context.KnittingSupplyItems
            .SingleOrDefaultAsync(item => item.Id == supplyId && item.UserId == userId);
        if (entity == null)
        {
            return null;
        }

        entity.Category = string.IsNullOrWhiteSpace(updateDto.Category)
            ? entity.Category
            : updateDto.Category.Trim();
        entity.Name = string.IsNullOrWhiteSpace(updateDto.Name)
            ? entity.Name
            : updateDto.Name.Trim();
        entity.Update(updateDto.Brand, updateDto.Color, updateDto.Size, updateDto.Quantity, updateDto.Unit, updateDto.Notes);

        await _context.SaveChangesAsync();
        return MapLegacySupply(entity);
    }

    public async Task<bool> DeleteSupplyAsync(Guid supplyId, Guid userId)
    {
        var entity = await _context.KnittingSupplyItems
            .SingleOrDefaultAsync(item => item.Id == supplyId && item.UserId == userId);
        if (entity == null)
        {
            return false;
        }

        _context.KnittingSupplyItems.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static KnittingSupplyItemDto MapLegacySupply(KnittingSupplyItem entity)
    {
        return new KnittingSupplyItemDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Name = entity.Name,
            Brand = entity.Brand,
            Color = entity.Color,
            Size = entity.Size,
            Quantity = entity.Quantity,
            Unit = entity.Unit,
            Notes = entity.Notes,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
