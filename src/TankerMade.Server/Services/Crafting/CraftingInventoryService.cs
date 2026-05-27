using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Crafting.DTOs.Inventory;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.ReferenceData;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;
using TankerMade.Server.Services.ReferenceData;

namespace TankerMade.Server.Services.Crafting;

public class CraftingInventoryService : ICraftingInventoryService
{
    private readonly TankerMadeDbContext _context;

    public CraftingInventoryService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<CraftingYarnInventoryItemDto> CreateOrMergeYarnAsync(
        CreateCraftingYarnInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateYarn(createDto);

        var normalizedBrand = CraftingYarnInventoryItem.Normalize(createDto.BrandName);
        var normalizedColor = CraftingYarnInventoryItem.Normalize(createDto.ColorName);

        var yarn = await _context.CraftingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.UserId == userId
                && y.NormalizedBrandName == normalizedBrand
                && y.NormalizedColorName == normalizedColor);

        if (yarn == null)
        {
            yarn = new CraftingYarnInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.ColorName);

            _context.CraftingYarnInventoryItems.Add(yarn);
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

    public async Task<CraftingYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId)
    {
        var yarn = await _context.CraftingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == id && y.UserId == userId);

        return yarn == null ? null : await MapYarnAsync(yarn.Id);
    }

    public async Task<IReadOnlyList<CraftingYarnInventoryItemDto>> GetYarnsAsync(
        Guid userId,
        CraftingYarnInventoryFilterDto? filter = null)
    {
        var query = ApplyYarnFilters(
            _context.CraftingYarnInventoryItems.Where(y => y.UserId == userId),
            filter);

        var yarns = await query
            .OrderBy(y => y.BrandName)
            .ThenBy(y => y.ColorName)
            .ToListAsync();

        var result = new List<CraftingYarnInventoryItemDto>();
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

    public async Task<CraftingToolInventoryItemDto> CreateOrMergeToolAsync(
        CreateCraftingToolInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateTool(createDto);

        var normalizedBrand = CraftingToolInventoryItem.Normalize(createDto.BrandName);
        var normalizedType = CraftingToolInventoryItem.Normalize(createDto.TypeName);

        var tool = await _context.CraftingToolInventoryItems
            .SingleOrDefaultAsync(t => t.UserId == userId
                && t.NormalizedBrandName == normalizedBrand
                && t.NormalizedTypeName == normalizedType);

        if (tool == null)
        {
            tool = new CraftingToolInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.TypeName);

            _context.CraftingToolInventoryItems.Add(tool);
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

    public async Task<CraftingToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId)
    {
        var tool = await _context.CraftingToolInventoryItems
            .SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        return tool == null ? null : await MapToolAsync(tool.Id);
    }

    public async Task<IReadOnlyList<CraftingToolInventoryItemDto>> GetToolsAsync(
        Guid userId,
        CraftingToolInventoryFilterDto? filter = null)
    {
        var query = ApplyToolFilters(
            _context.CraftingToolInventoryItems.Where(t => t.UserId == userId),
            filter);

        var tools = await query
            .OrderBy(t => t.TypeName)
            .ThenBy(t => t.BrandName)
            .ThenBy(t => t.Size)
            .ToListAsync();

        var result = new List<CraftingToolInventoryItemDto>();
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

    public async Task<CraftingNotionInventoryItemDto> CreateOrMergeNotionAsync(
        CreateCraftingNotionInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateNotion(createDto);

        var normalizedBrand = CraftingNotionInventoryItem.Normalize(createDto.BrandName);
        var normalizedType = CraftingNotionInventoryItem.Normalize(createDto.TypeName);

        var notion = await _context.CraftingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.UserId == userId
                && n.NormalizedBrandName == normalizedBrand
                && n.NormalizedTypeName == normalizedType);

        if (notion == null)
        {
            notion = new CraftingNotionInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.BrandName,
                createDto.TypeName);

            _context.CraftingNotionInventoryItems.Add(notion);
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

    public async Task<CraftingNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId)
    {
        var notion = await _context.CraftingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        return notion == null ? null : await MapNotionAsync(notion.Id);
    }

    public async Task<IReadOnlyList<CraftingNotionInventoryItemDto>> GetNotionsAsync(
        Guid userId,
        CraftingNotionInventoryFilterDto? filter = null)
    {
        var query = ApplyNotionFilters(
            _context.CraftingNotionInventoryItems.Where(n => n.UserId == userId),
            filter);

        var notions = await query
            .OrderBy(n => n.TypeName)
            .ThenBy(n => n.BrandName)
            .ThenBy(n => n.Size)
            .ThenBy(n => n.ColorName)
            .ToListAsync();

        var result = new List<CraftingNotionInventoryItemDto>();
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

    public async Task<IReadOnlyList<CraftingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category)
    {
        var coreReferenceItems = await CoreReferenceLookup.TryGetItemsAsync(_context, category);
        if (coreReferenceItems != null)
        {
            return coreReferenceItems
                .Select((item, index) => new CraftingInventoryReferenceItemDto
                {
                    Id = item.Id,
                    Category = item.Category,
                    Name = item.Name,
                    Slug = item.Slug,
                    SortOrder = index + 1
                })
                .ToList();
        }

        if (!CraftingReferenceCategories.IsModuleOwned(category))
        {
            return [];
        }

        var normalizedCategory = category.Trim();

        return await _context.CraftingInventoryReferenceItems
            .Where(i => i.Category == normalizedCategory)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Name)
            .Select(i => new CraftingInventoryReferenceItemDto
            {
                Id = i.Id,
                Category = i.Category,
                Name = i.Name,
                Slug = i.Slug,
                SortOrder = i.SortOrder
            })
            .ToListAsync();
    }

    public async Task<CraftingInventoryReferenceItemDto> CreateReferenceItemAsync(
        string category,
        CreateCraftingInventoryReferenceItemDto request)
    {
        if (!CraftingReferenceCategories.IsModuleOwned(category))
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
        var existing = await _context.CraftingInventoryReferenceItems
            .SingleOrDefaultAsync(i => i.Category == normalizedCategory && i.Slug == slug);

        if (existing != null)
        {
            return new CraftingInventoryReferenceItemDto
            {
                Id = existing.Id,
                Category = existing.Category,
                Name = existing.Name,
                Slug = existing.Slug,
                SortOrder = existing.SortOrder
            };
        }

        var maxSortOrder = await _context.CraftingInventoryReferenceItems
            .Where(i => i.Category == normalizedCategory)
            .Select(i => (int?)i.SortOrder)
            .MaxAsync() ?? 0;

        var created = new CraftingInventoryReferenceItem(
            Guid.NewGuid(),
            normalizedCategory,
            name,
            maxSortOrder + 1);

        _context.CraftingInventoryReferenceItems.Add(created);
        await _context.SaveChangesAsync();

        return new CraftingInventoryReferenceItemDto
        {
            Id = created.Id,
            Category = created.Category,
            Name = created.Name,
            Slug = created.Slug,
            SortOrder = created.SortOrder
        };
    }

    private IQueryable<CraftingYarnInventoryItem> ApplyYarnFilters(
        IQueryable<CraftingYarnInventoryItem> query,
        CraftingYarnInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = CraftingYarnInventoryItem.Normalize(filter.BrandName);
            query = query.Where(y => y.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.ColorName))
        {
            var normalized = CraftingYarnInventoryItem.Normalize(filter.ColorName);
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
            query = query.Where(y => _context.CraftingInventoryPurchases
                .Any(p => p.YarnInventoryItemId == y.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = CraftingYarnInventoryItem.Normalize(value);
            query = query.Where(y => y.NormalizedBrandName.Contains(normalized)
                || y.NormalizedColorName.Contains(normalized)
                || y.MainColor.Contains(value)
                || y.WeightName.Contains(value)
                || y.FiberContent.Contains(value)
                || y.FiberTag.Contains(value));
        }

        return query;
    }

    private IQueryable<CraftingToolInventoryItem> ApplyToolFilters(
        IQueryable<CraftingToolInventoryItem> query,
        CraftingToolInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = CraftingToolInventoryItem.Normalize(filter.BrandName);
            query = query.Where(t => t.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.TypeName))
        {
            var normalized = CraftingToolInventoryItem.Normalize(filter.TypeName);
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
            query = query.Where(t => _context.CraftingToolPurchases
                .Any(p => p.ToolInventoryItemId == t.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = CraftingToolInventoryItem.Normalize(value);
            query = query.Where(t => t.NormalizedBrandName.Contains(normalized)
                || t.NormalizedTypeName.Contains(normalized)
                || t.Size.Contains(value)
                || t.Description.Contains(value));
        }

        return query;
    }

    private IQueryable<CraftingNotionInventoryItem> ApplyNotionFilters(
        IQueryable<CraftingNotionInventoryItem> query,
        CraftingNotionInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = CraftingNotionInventoryItem.Normalize(filter.BrandName);
            query = query.Where(n => n.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.TypeName))
        {
            var normalized = CraftingNotionInventoryItem.Normalize(filter.TypeName);
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
            query = query.Where(n => _context.CraftingNotionPurchases
                .Any(p => p.NotionInventoryItemId == n.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = CraftingNotionInventoryItem.Normalize(value);
            query = query.Where(n => n.NormalizedBrandName.Contains(normalized)
                || n.NormalizedTypeName.Contains(normalized)
                || n.Size.Contains(value)
                || n.ColorName.Contains(value)
                || n.Description.Contains(value));
        }

        return query;
    }

    private async Task MergeLotAsync(CraftingYarnInventoryItem yarn, CreateCraftingYarnInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.LotNumber))
        {
            return;
        }

        var lotNumber = createDto.LotNumber.Trim();
        var lot = await _context.CraftingYarnLots
            .SingleOrDefaultAsync(l => l.YarnInventoryItemId == yarn.Id && l.LotNumber == lotNumber);

        if (lot == null)
        {
            _context.CraftingYarnLots.Add(new CraftingYarnLot(
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

    private void AddPurchase(CraftingYarnInventoryItem yarn, CreateCraftingYarnInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.CraftingInventoryPurchases.Add(new CraftingInventoryPurchase(
            Guid.NewGuid(),
            yarn.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private void AddToolPurchase(CraftingToolInventoryItem tool, CreateCraftingToolInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.CraftingToolPurchases.Add(new CraftingToolPurchase(
            Guid.NewGuid(),
            tool.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private void AddNotionPurchase(CraftingNotionInventoryItem notion, CreateCraftingNotionInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.CraftingNotionPurchases.Add(new CraftingNotionPurchase(
            Guid.NewGuid(),
            notion.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private static void ValidateCreateYarn(CreateCraftingYarnInventoryItemDto createDto)
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

    private static void ValidateCreateTool(CreateCraftingToolInventoryItemDto createDto)
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

    private static void ValidateCreateNotion(CreateCraftingNotionInventoryItemDto createDto)
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

    private async Task<CraftingYarnInventoryItemDto?> MapYarnAsync(Guid id)
    {
        var yarn = await _context.CraftingYarnInventoryItems
            .SingleOrDefaultAsync(y => y.Id == id);

        if (yarn == null)
        {
            return null;
        }

        var lots = await _context.CraftingYarnLots
            .Where(l => l.YarnInventoryItemId == id)
            .OrderBy(l => l.LotNumber)
            .Select(l => new CraftingYarnLotDto
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

        var purchases = await _context.CraftingInventoryPurchases
            .Where(p => p.YarnInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new CraftingInventoryPurchaseDto
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

        return new CraftingYarnInventoryItemDto
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

    private async Task<CraftingToolInventoryItemDto?> MapToolAsync(Guid id)
    {
        var tool = await _context.CraftingToolInventoryItems
            .SingleOrDefaultAsync(t => t.Id == id);

        if (tool == null)
        {
            return null;
        }

        var purchases = await _context.CraftingToolPurchases
            .Where(p => p.ToolInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new CraftingToolPurchaseDto
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

        return new CraftingToolInventoryItemDto
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

    private async Task<CraftingNotionInventoryItemDto?> MapNotionAsync(Guid id)
    {
        var notion = await _context.CraftingNotionInventoryItems
            .SingleOrDefaultAsync(n => n.Id == id);

        if (notion == null)
        {
            return null;
        }

        var purchases = await _context.CraftingNotionPurchases
            .Where(p => p.NotionInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new CraftingNotionPurchaseDto
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

        return new CraftingNotionInventoryItemDto
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
}
