using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Knitting.DTOs.Patterns;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Knitting;

public class KnittingPatternService : IKnittingPatternService
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private readonly TankerMadeDbContext _context;

    public KnittingPatternService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<KnittingPatternDto> CreateAsync(CreateKnittingPatternDto createDto, Guid userId)
    {
        var pattern = new KnittingPattern(Guid.NewGuid(), createDto.Name, userId);
        pattern.Update(createDto.Name, createDto.Type, createDto.Form, createDto.Difficulty, createDto.ThemeId, createDto.SourceId);

        _context.KnittingPatterns.Add(pattern);
        await _context.SaveChangesAsync();

        return await MapAsync(pattern);
    }

    public async Task<KnittingPatternDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var pattern = await _context.KnittingPatterns
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        return pattern == null ? null : await MapAsync(pattern);
    }

    public async Task<IReadOnlyList<KnittingPatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = DefaultPageSize)
    {
        var (skip, take) = ResolvePaging(page, pageSize);
        var patterns = await _context.KnittingPatterns
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return await MapListAsync(patterns);
    }

    public async Task<IReadOnlyList<KnittingPatternDto>> SearchAsync(string searchTerm, Guid userId, int page = 1, int pageSize = DefaultPageSize)
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

        var query = _context.KnittingPatterns
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

    public async Task<KnittingPatternDto?> UpdateAsync(UpdateKnittingPatternDto updateDto, Guid userId)
    {
        var pattern = await _context.KnittingPatterns
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
        var pattern = await _context.KnittingPatterns
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (pattern == null)
        {
            return false;
        }

        _context.KnittingPatterns.Remove(pattern);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<KnittingPatternPieceDto?> AddPieceAsync(Guid patternId, CreateKnittingPatternPieceDto createDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return null;
        }

        var nextSortOrder = await _context.KnittingPatternPieces
            .Where(p => p.PatternId == patternId)
            .Select(p => (int?)p.SortOrder)
            .MaxAsync() ?? 0;

        var piece = new KnittingPatternPiece(Guid.NewGuid(), patternId, createDto.Name, nextSortOrder + 1);
        _context.KnittingPatternPieces.Add(piece);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapPieceAsync(piece.Id);
    }

    public async Task<KnittingPatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateKnittingPatternPieceDto updateDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return null;
        }

        var piece = await _context.KnittingPatternPieces
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

        var piece = await _context.KnittingPatternPieces
            .SingleOrDefaultAsync(p => p.Id == pieceId && p.PatternId == patternId);

        if (piece == null)
        {
            return false;
        }

        _context.KnittingPatternPieces.Remove(piece);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();
        await NormalizePieceOrderAsync(patternId);
        return true;
    }

    public async Task<bool> ReorderPiecesAsync(Guid patternId, ReorderKnittingPatternItemsDto reorderDto, Guid userId)
    {
        if (!await PatternExistsAsync(patternId, userId))
        {
            return false;
        }

        var pieces = await _context.KnittingPatternPieces
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

    public async Task<KnittingPatternStepDto?> AddStepAsync(Guid patternId, Guid pieceId, CreateKnittingPatternStepDto createDto, Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return null;
        }

        var nextSortOrder = await _context.KnittingPatternSteps
            .Where(s => s.PatternPieceId == pieceId)
            .Select(s => (int?)s.SortOrder)
            .MaxAsync() ?? 0;

        var step = new KnittingPatternStep(
            Guid.NewGuid(),
            pieceId,
            createDto.RangeStart,
            createDto.RangeEnd,
            createDto.Label,
            createDto.Instructions,
            nextSortOrder + 1);

        _context.KnittingPatternSteps.Add(step);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();

        return await MapStepAsync(step.Id);
    }

    public async Task<KnittingPatternStepDto?> UpdateStepAsync(Guid patternId, Guid pieceId, UpdateKnittingPatternStepDto updateDto, Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return null;
        }

        var step = await _context.KnittingPatternSteps
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

        var step = await _context.KnittingPatternSteps
            .SingleOrDefaultAsync(s => s.Id == stepId && s.PatternPieceId == pieceId);

        if (step == null)
        {
            return false;
        }

        _context.KnittingPatternSteps.Remove(step);
        await TouchPatternAsync(patternId);
        await _context.SaveChangesAsync();
        await NormalizeStepOrderAsync(pieceId);
        return true;
    }

    public async Task<bool> ReorderStepsAsync(Guid patternId, Guid pieceId, ReorderKnittingPatternItemsDto reorderDto, Guid userId)
    {
        if (!await PieceBelongsToUserPatternAsync(patternId, pieceId, userId))
        {
            return false;
        }

        var steps = await _context.KnittingPatternSteps
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

    private async Task<KnittingPatternDto> MapAsync(KnittingPattern pattern)
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

        return new KnittingPatternDto
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
            StepCount = pieces.Sum(p => p.Steps.Count),
            CreatedAt = pattern.CreatedAt,
            UpdatedAt = pattern.UpdatedAt
        };
    }

    private async Task<IReadOnlyList<KnittingPatternDto>> MapListAsync(IReadOnlyList<KnittingPattern> patterns)
    {
        var results = new List<KnittingPatternDto>(patterns.Count);
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
        return await _context.KnittingPatterns
            .AnyAsync(p => p.Id == patternId && p.UserId == userId);
    }

    private async Task<bool> PieceBelongsToUserPatternAsync(Guid patternId, Guid pieceId, Guid userId)
    {
        return await _context.KnittingPatternPieces
            .AnyAsync(piece => piece.Id == pieceId
                && piece.PatternId == patternId
                && _context.KnittingPatterns.Any(pattern => pattern.Id == patternId && pattern.UserId == userId));
    }

    private async Task TouchPatternAsync(Guid patternId)
    {
        var pattern = await _context.KnittingPatterns.SingleAsync(p => p.Id == patternId);
        pattern.UpdatedAt = DateTime.UtcNow;
    }

    private async Task NormalizePieceOrderAsync(Guid patternId)
    {
        var pieces = await _context.KnittingPatternPieces
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
        var steps = await _context.KnittingPatternSteps
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

    private async Task<IReadOnlyList<KnittingPatternPieceDto>> MapPiecesAsync(Guid patternId)
    {
        var pieces = await _context.KnittingPatternPieces
            .Where(p => p.PatternId == patternId)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();

        var results = new List<KnittingPatternPieceDto>(pieces.Count);
        foreach (var piece in pieces)
        {
            results.Add(await MapPiece(piece));
        }

        return results;
    }

    private async Task<KnittingPatternPieceDto?> MapPieceAsync(Guid pieceId)
    {
        var piece = await _context.KnittingPatternPieces
            .SingleOrDefaultAsync(p => p.Id == pieceId);

        return piece == null ? null : await MapPiece(piece);
    }

    private async Task<KnittingPatternPieceDto> MapPiece(KnittingPatternPiece piece)
    {
        var steps = await _context.KnittingPatternSteps
            .Where(s => s.PatternPieceId == piece.Id)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.CreatedAt)
            .ToListAsync();

        return new KnittingPatternPieceDto
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

    private async Task<KnittingPatternStepDto?> MapStepAsync(Guid stepId)
    {
        var step = await _context.KnittingPatternSteps
            .SingleOrDefaultAsync(s => s.Id == stepId);

        return step == null ? null : MapStep(step);
    }

    private static KnittingPatternStepDto MapStep(KnittingPatternStep step)
    {
        return new KnittingPatternStepDto
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
