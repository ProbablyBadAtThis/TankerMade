using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Printing3D.DTOs.Inventory;
using TankerMade.Modules.Printing3D.Entities;
using TankerMade.Modules.Printing3D.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Printing3D;

public class PrintingInventoryService : IPrintingInventoryService
{
    private readonly TankerMadeDbContext _context;

    public PrintingInventoryService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<PrintingMaterialInventoryItemDto> CreateOrMergeMaterialAsync(
        CreatePrintingMaterialInventoryItemDto createDto,
        Guid userId)
    {
        ValidateCreateMaterial(createDto);

        var normalizedMaterial = PrintingMaterialInventoryItem.Normalize(createDto.MaterialType);
        var normalizedBrand = PrintingMaterialInventoryItem.Normalize(createDto.BrandName);
        var normalizedColor = PrintingMaterialInventoryItem.Normalize(createDto.ColorName);

        var material = await _context.PrintingMaterialInventoryItems
            .SingleOrDefaultAsync(m => m.UserId == userId
                && m.NormalizedMaterialType == normalizedMaterial
                && m.NormalizedBrandName == normalizedBrand
                && m.NormalizedColorName == normalizedColor);

        if (material == null)
        {
            material = new PrintingMaterialInventoryItem(
                Guid.NewGuid(),
                userId,
                createDto.MaterialType,
                createDto.BrandName,
                createDto.ColorName);

            _context.PrintingMaterialInventoryItems.Add(material);
        }

        material.MergeDetails(
            createDto.SpoolWeightGrams,
            createDto.RemainingWeightGrams,
            createDto.Diameter,
            createDto.StorageLocation,
            createDto.Price,
            createDto.IsSalePrice);

        await MergeSpoolAsync(material, createDto);
        AddPurchase(material, createDto);

        await _context.SaveChangesAsync();
        return await MapMaterialAsync(material.Id) ?? throw new InvalidOperationException("Created material could not be loaded.");
    }

    public async Task<PrintingMaterialInventoryItemDto?> GetMaterialByIdAsync(Guid id, Guid userId)
    {
        var material = await _context.PrintingMaterialInventoryItems
            .SingleOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        return material == null ? null : await MapMaterialAsync(material.Id);
    }

    public async Task<IReadOnlyList<PrintingMaterialInventoryItemDto>> GetMaterialsAsync(
        Guid userId,
        PrintingMaterialInventoryFilterDto? filter = null)
    {
        var query = ApplyMaterialFilters(
            _context.PrintingMaterialInventoryItems.Where(m => m.UserId == userId),
            filter);

        var materials = await query
            .OrderBy(m => m.MaterialType)
            .ThenBy(m => m.BrandName)
            .ThenBy(m => m.ColorName)
            .ToListAsync();

        var result = new List<PrintingMaterialInventoryItemDto>();
        foreach (var material in materials)
        {
            var dto = await MapMaterialAsync(material.Id);
            if (dto != null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    public async Task<IReadOnlyList<PrintingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category)
    {
        var normalizedCategory = category.Trim();

        return await _context.PrintingInventoryReferenceItems
            .Where(i => i.Category == normalizedCategory)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Name)
            .Select(i => new PrintingInventoryReferenceItemDto
            {
                Id = i.Id,
                Category = i.Category,
                Name = i.Name,
                Slug = i.Slug,
                SortOrder = i.SortOrder
            })
            .ToListAsync();
    }

    private IQueryable<PrintingMaterialInventoryItem> ApplyMaterialFilters(
        IQueryable<PrintingMaterialInventoryItem> query,
        PrintingMaterialInventoryFilterDto? filter)
    {
        if (filter == null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.MaterialType))
        {
            var normalized = PrintingMaterialInventoryItem.Normalize(filter.MaterialType);
            query = query.Where(m => m.NormalizedMaterialType.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.BrandName))
        {
            var normalized = PrintingMaterialInventoryItem.Normalize(filter.BrandName);
            query = query.Where(m => m.NormalizedBrandName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.ColorName))
        {
            var normalized = PrintingMaterialInventoryItem.Normalize(filter.ColorName);
            query = query.Where(m => m.NormalizedColorName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(filter.Diameter))
        {
            var value = filter.Diameter.Trim();
            query = query.Where(m => m.Diameter.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.StorageLocation))
        {
            var value = filter.StorageLocation.Trim();
            query = query.Where(m => m.StorageLocation.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.SourceName))
        {
            var value = filter.SourceName.Trim();
            query = query.Where(m => _context.PrintingInventoryPurchases
                .Any(p => p.MaterialInventoryItemId == m.Id && p.SourceName.Contains(value)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var value = filter.Search.Trim();
            var normalized = PrintingMaterialInventoryItem.Normalize(value);
            query = query.Where(m => m.NormalizedMaterialType.Contains(normalized)
                || m.NormalizedBrandName.Contains(normalized)
                || m.NormalizedColorName.Contains(normalized)
                || m.Diameter.Contains(value)
                || m.StorageLocation.Contains(value));
        }

        return query;
    }

    private async Task MergeSpoolAsync(
        PrintingMaterialInventoryItem material,
        CreatePrintingMaterialInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SpoolCode))
        {
            return;
        }

        var spoolCode = createDto.SpoolCode.Trim();
        var spool = await _context.PrintingSpools
            .SingleOrDefaultAsync(s => s.MaterialInventoryItemId == material.Id && s.SpoolCode == spoolCode);

        if (spool == null)
        {
            _context.PrintingSpools.Add(new PrintingSpool(
                Guid.NewGuid(),
                material.Id,
                spoolCode,
                createDto.SpoolWeightGrams,
                createDto.RemainingWeightGrams,
                createDto.PrinterCompatibility));
            return;
        }

        spool.Merge(createDto.SpoolWeightGrams, createDto.RemainingWeightGrams, createDto.PrinterCompatibility);
    }

    private void AddPurchase(
        PrintingMaterialInventoryItem material,
        CreatePrintingMaterialInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.SourceName)
            && !createDto.Price.HasValue
            && !createDto.PurchasedAt.HasValue)
        {
            return;
        }

        _context.PrintingInventoryPurchases.Add(new PrintingInventoryPurchase(
            Guid.NewGuid(),
            material.Id,
            createDto.SourceName,
            createDto.Price,
            createDto.IsSalePrice,
            createDto.PurchasedAt));
    }

    private static void ValidateCreateMaterial(CreatePrintingMaterialInventoryItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.MaterialType))
        {
            throw new ArgumentException("Material type is required.", nameof(createDto));
        }

        if (string.IsNullOrWhiteSpace(createDto.BrandName))
        {
            throw new ArgumentException("Brand name is required.", nameof(createDto));
        }

        if (string.IsNullOrWhiteSpace(createDto.ColorName))
        {
            throw new ArgumentException("Color name is required.", nameof(createDto));
        }

        if (createDto.SpoolWeightGrams <= 0)
        {
            throw new ArgumentException("Spool weight must be greater than zero.", nameof(createDto));
        }
    }

    private async Task<PrintingMaterialInventoryItemDto?> MapMaterialAsync(Guid id)
    {
        var material = await _context.PrintingMaterialInventoryItems
            .SingleOrDefaultAsync(m => m.Id == id);

        if (material == null)
        {
            return null;
        }

        var spools = await _context.PrintingSpools
            .Where(s => s.MaterialInventoryItemId == id)
            .OrderBy(s => s.SpoolCode)
            .Select(s => new PrintingSpoolDto
            {
                Id = s.Id,
                MaterialInventoryItemId = s.MaterialInventoryItemId,
                SpoolCode = s.SpoolCode,
                StartingWeightGrams = s.StartingWeightGrams,
                RemainingWeightGrams = s.RemainingWeightGrams,
                PrinterCompatibility = s.PrinterCompatibility,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        var purchases = await _context.PrintingInventoryPurchases
            .Where(p => p.MaterialInventoryItemId == id)
            .OrderByDescending(p => p.PurchasedAt ?? p.CreatedAt)
            .Select(p => new PrintingInventoryPurchaseDto
            {
                Id = p.Id,
                MaterialInventoryItemId = p.MaterialInventoryItemId,
                SourceName = p.SourceName,
                Price = p.Price,
                IsSalePrice = p.IsSalePrice,
                PurchasedAt = p.PurchasedAt,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new PrintingMaterialInventoryItemDto
        {
            Id = material.Id,
            UserId = material.UserId,
            MaterialType = material.MaterialType,
            BrandName = material.BrandName,
            ColorName = material.ColorName,
            TotalSpoolWeightGrams = material.TotalSpoolWeightGrams,
            RemainingWeightGrams = material.RemainingWeightGrams,
            Diameter = material.Diameter,
            StorageLocation = material.StorageLocation,
            RegularPrice = material.RegularPrice,
            Spools = spools,
            Purchases = purchases,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt
        };
    }
}
