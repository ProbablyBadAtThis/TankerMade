using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Knitting.DTOs.Inventory;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Knitting;

public class KnittingInventoryService : IKnittingInventoryService
{
    private readonly TankerMadeDbContext _context;

    public KnittingInventoryService(TankerMadeDbContext context)
    {
        _context = context;
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

        return items.Select(Map).ToList();
    }

    public async Task<KnittingSupplyItemDto> CreateSupplyAsync(CreateKnittingSupplyItemDto createDto, Guid userId)
    {
        var entity = new KnittingSupplyItem(Guid.NewGuid(), userId, createDto.Category, createDto.Name);
        entity.Update(createDto.Brand, createDto.Color, createDto.Size, createDto.Quantity, createDto.Unit, createDto.Notes);

        _context.KnittingSupplyItems.Add(entity);
        await _context.SaveChangesAsync();

        return Map(entity);
    }

    private static KnittingSupplyItemDto Map(KnittingSupplyItem entity)
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
