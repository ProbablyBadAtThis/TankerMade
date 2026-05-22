using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Crafting;

public class CraftingPatternService : ICraftingPatternService
{
    private readonly TankerMadeDbContext _context;

    public CraftingPatternService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<CraftingPatternDto> CreateAsync(CreateCraftingPatternDto createDto, Guid userId)
    {
        var pattern = new CraftingPattern(Guid.NewGuid(), createDto.Name, userId);
        pattern.Update(createDto.Name, createDto.Type, createDto.Form, createDto.Difficulty, createDto.ThemeId, createDto.SourceId);

        _context.CraftingPatterns.Add(pattern);
        await _context.SaveChangesAsync();

        return await MapAsync(pattern);
    }

    public async Task<CraftingPatternDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var pattern = await _context.CraftingPatterns
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        return pattern == null ? null : await MapAsync(pattern);
    }

    public async Task<CraftingPatternDto?> GetBySlugAsync(string slug, Guid userId)
    {
        var pattern = await _context.CraftingPatterns
            .SingleOrDefaultAsync(p => p.Slug == slug && p.UserId == userId);

        return pattern == null ? null : await MapAsync(pattern);
    }

    public async Task<IReadOnlyList<CraftingPatternDto>> GetAllAsync(Guid userId)
    {
        var patterns = await _context.CraftingPatterns
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(patterns);
    }

    public async Task<IReadOnlyList<CraftingPatternDto>> GetByThemeIdAsync(Guid themeId, Guid userId)
    {
        var patterns = await _context.CraftingPatterns
            .Where(p => p.UserId == userId && p.ThemeId == themeId)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(patterns);
    }

    public async Task<IReadOnlyList<CraftingPatternDto>> SearchAsync(string searchTerm, Guid userId)
    {
        var normalized = searchTerm.Trim();
        var patterns = await _context.CraftingPatterns
            .Where(p => p.UserId == userId && (p.Name.Contains(normalized) || p.Type.Contains(normalized)))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(patterns);
    }

    public async Task<CraftingPatternDto?> UpdateAsync(UpdateCraftingPatternDto updateDto, Guid userId)
    {
        var pattern = await _context.CraftingPatterns
            .SingleOrDefaultAsync(p => p.Id == updateDto.Id && p.UserId == userId);

        if (pattern == null)
        {
            return null;
        }

        pattern.Update(
            UseIncomingValue(updateDto.Name, pattern.Name),
            UseIncomingValue(updateDto.Type, pattern.Type),
            UseIncomingValue(updateDto.Form, pattern.Form),
            UseIncomingValue(updateDto.Difficulty, pattern.Difficulty),
            updateDto.ThemeId ?? pattern.ThemeId,
            updateDto.SourceId ?? pattern.SourceId);
        await _context.SaveChangesAsync();

        return await MapAsync(pattern);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var pattern = await _context.CraftingPatterns
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (pattern == null)
        {
            return false;
        }

        _context.CraftingPatterns.Remove(pattern);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<CraftingPatternDto> MapAsync(CraftingPattern pattern)
    {
        var userName = await _context.Users
            .Where(u => u.Id == pattern.UserId)
            .Select(u => u.Username)
            .SingleOrDefaultAsync() ?? string.Empty;

        var themeName = pattern.ThemeId == null
            ? string.Empty
            : await _context.Themes
                .Where(t => t.Id == pattern.ThemeId)
                .Select(t => t.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var sourceName = pattern.SourceId == null
            ? string.Empty
            : await _context.Sources
                .Where(s => s.Id == pattern.SourceId)
                .Select(s => s.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        return new CraftingPatternDto
        {
            Id = pattern.Id,
            Name = pattern.Name,
            Slug = pattern.Slug,
            Type = pattern.Type,
            Form = pattern.Form,
            Difficulty = pattern.Difficulty,
            ThemeId = pattern.ThemeId,
            ThemeName = themeName,
            SourceId = pattern.SourceId,
            SourceName = sourceName,
            UserId = pattern.UserId,
            Username = userName,
            CreatedAt = pattern.CreatedAt,
            UpdatedAt = pattern.UpdatedAt
        };
    }

    private async Task<IReadOnlyList<CraftingPatternDto>> MapListAsync(IReadOnlyList<CraftingPattern> patterns)
    {
        var results = new List<CraftingPatternDto>(patterns.Count);
        foreach (var pattern in patterns)
        {
            results.Add(await MapAsync(pattern));
        }

        return results;
    }

    private static string UseIncomingValue(string? incomingValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(incomingValue)
            ? currentValue
            : incomingValue.Trim();
    }
}
