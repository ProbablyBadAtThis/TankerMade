using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Crafting;

public class CraftingPatternService : ICraftingPatternService
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
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

    public async Task<IReadOnlyList<CraftingPatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = DefaultPageSize)
    {
        var (skip, take) = ResolvePaging(page, pageSize);
        var patterns = await _context.CraftingPatterns
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
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

    public async Task<IReadOnlyList<CraftingPatternDto>> SearchAsync(
        string searchTerm,
        Guid userId,
        int page = 1,
        int pageSize = DefaultPageSize)
    {
        var terms = searchTerm
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(term => term.ToLowerInvariant())
            .Distinct()
            .ToList();
        if (terms.Count == 0)
        {
            return [];
        }

        var query = _context.CraftingPatterns
            .Where(p => p.UserId == userId);
        foreach (var term in terms)
        {
            var like = $"%{term}%";
            query = query.Where(p =>
                EF.Functions.Like((p.Name ?? string.Empty).ToLower(), like)
                || EF.Functions.Like((p.Type ?? string.Empty).ToLower(), like));
        }

        var (skip, take) = ResolvePaging(page, pageSize);
        var patterns = await query
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
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

        await EnsurePatternHasNoProjectsAsync(id, userId, "delete this pattern");
        _context.CraftingPatterns.Remove(pattern);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CraftingPatternPieceDto?> AddPieceAsync(Guid patternId, CreateCraftingPatternPieceDto createDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return null;
        }

        var nextSortOrder = await _context.CraftingPatternPieces
            .Where(p => p.PatternId == patternId)
            .Select(p => (int?)p.SortOrder)
            .MaxAsync() ?? 0;

        var piece = new CraftingPatternPiece(Guid.NewGuid(), patternId, createDto.Name, nextSortOrder + 1);
        _context.CraftingPatternPieces.Add(piece);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapPieceAsync(piece.Id);
    }

    public async Task<CraftingPatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateCraftingPatternPieceDto updateDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return null;
        }

        var piece = await _context.CraftingPatternPieces
            .SingleOrDefaultAsync(p => p.Id == updateDto.Id && p.PatternId == patternId);

        if (piece == null)
        {
            return null;
        }

        piece.Update(updateDto.Name);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapPieceAsync(piece.Id);
    }

    public async Task<bool> DeletePieceAsync(Guid patternId, Guid pieceId, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return false;
        }

        var piece = await _context.CraftingPatternPieces
            .SingleOrDefaultAsync(p => p.Id == pieceId && p.PatternId == patternId);

        if (piece == null)
        {
            return false;
        }

        await EnsurePatternHasNoProjectsAsync(patternId, userId, "delete pattern pieces");
        _context.CraftingPatternPieces.Remove(piece);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();
        await NormalizePieceOrderAsync(patternId);
        return true;
    }

    public async Task<bool> ReorderPiecesAsync(Guid patternId, ReorderCraftingPatternItemsDto reorderDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return false;
        }

        var pieces = await _context.CraftingPatternPieces
            .Where(p => p.PatternId == patternId)
            .ToListAsync();

        if (!ContainsSameIds(pieces.Select(p => p.Id), reorderDto.OrderedIds))
        {
            return false;
        }

        for (var i = 0; i < reorderDto.OrderedIds.Count; i++)
        {
            pieces.Single(p => p.Id == reorderDto.OrderedIds[i]).MoveTo(i + 1);
        }

        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CraftingPatternStepDto?> AddStepAsync(
        Guid patternId,
        Guid pieceId,
        CreateCraftingPatternStepDto createDto,
        Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return null;
        }

        var nextSortOrder = await _context.CraftingPatternSteps
            .Where(s => s.PatternPieceId == pieceId)
            .Select(s => (int?)s.SortOrder)
            .MaxAsync() ?? 0;

        var step = new CraftingPatternStep(
            Guid.NewGuid(),
            pieceId,
            createDto.RangeStart,
            createDto.RangeEnd,
            createDto.Label,
            createDto.Instructions,
            nextSortOrder + 1);

        _context.CraftingPatternSteps.Add(step);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapStepAsync(step.Id);
    }

    public async Task<CraftingPatternStepDto?> UpdateStepAsync(
        Guid patternId,
        Guid pieceId,
        UpdateCraftingPatternStepDto updateDto,
        Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return null;
        }

        var step = await _context.CraftingPatternSteps
            .SingleOrDefaultAsync(s => s.Id == updateDto.Id && s.PatternPieceId == pieceId);

        if (step == null)
        {
            return null;
        }

        step.Update(updateDto.RangeStart, updateDto.RangeEnd, updateDto.Label, updateDto.Instructions);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapStepAsync(step.Id);
    }

    public async Task<bool> DeleteStepAsync(Guid patternId, Guid pieceId, Guid stepId, Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return false;
        }

        var step = await _context.CraftingPatternSteps
            .SingleOrDefaultAsync(s => s.Id == stepId && s.PatternPieceId == pieceId);

        if (step == null)
        {
            return false;
        }

        await EnsurePatternHasNoProjectsAsync(patternId, userId, "delete pattern steps");
        _context.CraftingPatternSteps.Remove(step);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();
        await NormalizeStepOrderAsync(pieceId);
        return true;
    }

    public async Task<bool> ReorderStepsAsync(
        Guid patternId,
        Guid pieceId,
        ReorderCraftingPatternItemsDto reorderDto,
        Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return false;
        }

        var steps = await _context.CraftingPatternSteps
            .Where(s => s.PatternPieceId == pieceId)
            .ToListAsync();

        if (!ContainsSameIds(steps.Select(s => s.Id), reorderDto.OrderedIds))
        {
            return false;
        }

        for (var i = 0; i < reorderDto.OrderedIds.Count; i++)
        {
            steps.Single(s => s.Id == reorderDto.OrderedIds[i]).MoveTo(i + 1);
        }

        await TouchPatternAsync(patternId);
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

        var pieces = await MapPiecesAsync(pattern.Id);
        var stepCount = pieces.Sum(p => p.Steps.Count);
        var progress = BuildProgress(pieces);

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
            Pieces = pieces,
            PieceCount = pieces.Count,
            StepCount = stepCount,
            Progress = progress,
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

    private async Task<bool> PatternExistsAsync(Guid patternId, Guid userId)
    {
        return await _context.CraftingPatterns
            .AnyAsync(p => p.Id == patternId && p.UserId == userId);
    }

    private async Task<bool> PieceBelongsToUserPatternAsync(Guid patternId, Guid pieceId, Guid userId)
    {
        return await _context.CraftingPatternPieces
            .AnyAsync(piece => piece.Id == pieceId
                && piece.PatternId == patternId
                && _context.CraftingPatterns.Any(pattern => pattern.Id == patternId && pattern.UserId == userId));
    }

    private async Task TouchPatternAsync(Guid patternId)
    {
        var pattern = await _context.CraftingPatterns.SingleAsync(p => p.Id == patternId);
        pattern.UpdatedAt = DateTime.UtcNow;
    }

    private async Task EnsurePatternHasNoProjectsAsync(Guid patternId, Guid userId, string action)
    {
        var isUsed = await _context.CraftingProjects
            .AnyAsync(project => project.PatternId == patternId && project.UserId == userId);

        if (isUsed)
        {
            throw new InvalidOperationException($"Cannot {action} while one or more projects are linked to this pattern. Edit names, instructions, or ordering instead, or create a new pattern version.");
        }
    }

    private async Task NormalizePieceOrderAsync(Guid patternId)
    {
        var pieces = await _context.CraftingPatternPieces
            .Where(p => p.PatternId == patternId)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();

        for (var i = 0; i < pieces.Count; i++)
        {
            pieces[i].MoveTo(i + 1);
        }

        await _context.SaveChangesAsync();
    }

    private async Task NormalizeStepOrderAsync(Guid pieceId)
    {
        var steps = await _context.CraftingPatternSteps
            .Where(s => s.PatternPieceId == pieceId)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.CreatedAt)
            .ToListAsync();

        for (var i = 0; i < steps.Count; i++)
        {
            steps[i].MoveTo(i + 1);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<CraftingPatternPieceDto>> MapPiecesAsync(Guid patternId)
    {
        var pieces = await _context.CraftingPatternPieces
            .Where(p => p.PatternId == patternId)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();

        var results = new List<CraftingPatternPieceDto>(pieces.Count);
        foreach (var piece in pieces)
        {
            results.Add(await MapPiece(piece));
        }

        return results;
    }

    private async Task<CraftingPatternPieceDto?> MapPieceAsync(Guid pieceId)
    {
        var piece = await _context.CraftingPatternPieces
            .SingleOrDefaultAsync(p => p.Id == pieceId);

        return piece == null ? null : await MapPiece(piece);
    }

    private async Task<CraftingPatternPieceDto> MapPiece(CraftingPatternPiece piece)
    {
        var steps = await _context.CraftingPatternSteps
            .Where(s => s.PatternPieceId == piece.Id)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.CreatedAt)
            .ToListAsync();

        return new CraftingPatternPieceDto
        {
            Id = piece.Id,
            PatternId = piece.PatternId,
            Name = piece.Name,
            SortOrder = piece.SortOrder,
            Steps = steps.Select(MapStep).ToList(),
            CreatedAt = piece.CreatedAt,
            UpdatedAt = piece.UpdatedAt
        };
    }

    private async Task<CraftingPatternStepDto?> MapStepAsync(Guid stepId)
    {
        var step = await _context.CraftingPatternSteps
            .SingleOrDefaultAsync(s => s.Id == stepId);

        return step == null ? null : MapStep(step);
    }

    private static CraftingPatternStepDto MapStep(CraftingPatternStep step)
    {
        return new CraftingPatternStepDto
        {
            Id = step.Id,
            PatternPieceId = step.PatternPieceId,
            RangeStart = step.RangeStart,
            RangeEnd = step.RangeEnd,
            DisplayRange = FormatRange(step.RangeStart, step.RangeEnd),
            Label = step.Label,
            Instructions = step.Instructions,
            SortOrder = step.SortOrder,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
        };
    }

    private static string FormatRange(int? rangeStart, int? rangeEnd)
    {
        if (rangeStart == null && rangeEnd == null)
        {
            return string.Empty;
        }

        if (rangeStart == rangeEnd || rangeEnd == null)
        {
            return rangeStart?.ToString() ?? string.Empty;
        }

        if (rangeStart == null)
        {
            return rangeEnd.Value.ToString();
        }

        return $"{rangeStart}-{rangeEnd}";
    }

    private static CraftingPatternProgressDto BuildProgress(IReadOnlyList<CraftingPatternPieceDto> pieces)
    {
        var stepCount = pieces.Sum(piece => piece.Steps.Count);
        var emptyPieceCount = pieces.Count(piece => piece.Steps.Count == 0);
        var invalidRangeCount = pieces
            .SelectMany(piece => piece.Steps)
            .Count(step => step.RangeStart.HasValue && step.RangeEnd.HasValue && step.RangeEnd.Value < step.RangeStart.Value);

        var messages = new List<string>();
        if (pieces.Count == 0)
        {
            messages.Add("Add at least one piece before using this pattern for a project.");
        }

        if (stepCount == 0)
        {
            messages.Add("Add at least one step before using this pattern for a project.");
        }

        if (emptyPieceCount > 0)
        {
            messages.Add($"{emptyPieceCount} piece(s) do not have steps yet.");
        }

        if (invalidRangeCount > 0)
        {
            messages.Add($"{invalidRangeCount} step range(s) need attention.");
        }

        return new CraftingPatternProgressDto
        {
            PieceCount = pieces.Count,
            StepCount = stepCount,
            EmptyPieceCount = emptyPieceCount,
            InvalidRangeCount = invalidRangeCount,
            ValidationMessages = messages
        };
    }

    private static bool ContainsSameIds(IEnumerable<Guid> currentIds, IReadOnlyList<Guid> orderedIds)
    {
        var currentSet = currentIds.ToHashSet();
        var orderedSet = orderedIds.ToHashSet();

        return currentSet.Count == orderedIds.Count
            && orderedSet.Count == orderedIds.Count
            && currentSet.SetEquals(orderedSet);
    }

    private static (int Skip, int Take) ResolvePaging(int page, int pageSize)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return ((safePage - 1) * safePageSize, safePageSize);
    }
}
