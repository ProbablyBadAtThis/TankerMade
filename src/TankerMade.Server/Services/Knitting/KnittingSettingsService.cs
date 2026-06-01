using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Knitting.DTOs.Settings;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Knitting;

public class KnittingSettingsService : IKnittingSettingsService
{
    private readonly TankerMadeDbContext _context;

    public KnittingSettingsService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KnittingSettingItemDto>> GetAllAsync(Guid userId, string? category = null)
    {
        var query = _context.KnittingSettingItems.Where(item => item.UserId == userId);
        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalized = category.Trim().ToLowerInvariant();
            query = query.Where(item => item.Category.ToLower() == normalized);
        }

        var items = await query
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Key)
            .ToListAsync();

        return items.Select(Map).ToList();
    }

    public async Task<KnittingSettingItemDto> UpsertAsync(UpsertKnittingSettingItemDto request, Guid userId)
    {
        var key = request.Key.Trim();
        var existing = await _context.KnittingSettingItems
            .SingleOrDefaultAsync(item => item.UserId == userId && item.Key == key);

        if (existing == null)
        {
            existing = new KnittingSettingItem(Guid.NewGuid(), userId, key);
            _context.KnittingSettingItems.Add(existing);
        }

        existing.Update(request.Value, request.Category);
        await _context.SaveChangesAsync();

        return Map(existing);
    }

    private static KnittingSettingItemDto Map(KnittingSettingItem item)
    {
        return new KnittingSettingItemDto
        {
            Id = item.Id,
            Key = item.Key,
            Value = item.Value,
            Category = item.Category,
            UserId = item.UserId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
