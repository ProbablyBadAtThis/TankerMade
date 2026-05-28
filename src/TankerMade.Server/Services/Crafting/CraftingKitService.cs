using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Enums;
using TankerMade.Modules.Crafting.DTOs.Kits;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Crafting;

public class CraftingKitService : ICraftingKitService
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private readonly TankerMadeDbContext _context;

    public CraftingKitService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<CraftingKitDto> CreateAsync(CreateCraftingKitDto createDto, Guid userId)
    {
        var kit = new CraftingKit(Guid.NewGuid(), createDto.Name, userId);
        kit.Update(createDto.Name, createDto.Description, createDto.Type, createDto.ThemeId, createDto.Difficulty, createDto.Progress);

        _context.CraftingKits.Add(kit);
        await _context.SaveChangesAsync();

        return await MapAsync(kit);
    }

    public async Task<CraftingKitDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == id && kit.UserId == userId);
        return kit == null ? null : await MapAsync(kit);
    }

    public async Task<IReadOnlyList<CraftingKitDto>> GetAllAsync(
        Guid userId,
        bool includeArchived = false,
        int page = 1,
        int pageSize = DefaultPageSize)
    {
        var (skip, take) = ResolvePaging(page, pageSize);
        var kits = await _context.CraftingKits
            .Where(kit => kit.UserId == userId && (includeArchived || !kit.IsArchived))
            .OrderBy(kit => kit.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return await MapListAsync(kits);
    }

    public async Task<CraftingKitDto?> UpdateAsync(UpdateCraftingKitDto updateDto, Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == updateDto.Id && kit.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        kit.Update(
            UseIncomingValue(updateDto.Name, kit.Name),
            UseIncomingValue(updateDto.Description, kit.Description),
            UseIncomingValue(updateDto.Type, kit.Type),
            updateDto.ThemeId ?? kit.ThemeId,
            updateDto.Difficulty ?? kit.Difficulty,
            updateDto.Progress ?? kit.Progress);

        await _context.SaveChangesAsync();
        return await MapAsync(kit);
    }

    public async Task<CraftingKitDto?> ArchiveAsync(Guid id, Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == id && kit.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        kit.Archive(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(kit);
    }

    public async Task<CraftingKitDto?> ReopenAsync(Guid id, Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == id && kit.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        kit.Reopen(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(kit);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == id && kit.UserId == userId);
        if (kit == null)
        {
            return false;
        }

        await ClearProjectKitLinksAsync(id);
        _context.CraftingKits.Remove(kit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CraftingKitPieceDto?> AddPieceAsync(Guid kitId, CreateCraftingKitPieceDto createDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return null;
        }

        await EnsurePatternBelongsToUserAsync(createDto.PatternId, userId);
        var nextSortOrder = await _context.CraftingKitPieces
            .Where(piece => piece.KitId == kitId)
            .Select(piece => (int?)piece.SortOrder)
            .MaxAsync() ?? 0;

        var piece = new CraftingKitPiece(Guid.NewGuid(), kitId, createDto.Name, createDto.PatternId, createDto.Notes, nextSortOrder + 1);
        _context.CraftingKitPieces.Add(piece);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();

        return await MapPieceAsync(piece.Id);
    }

    public async Task<CraftingKitPieceDto?> UpdatePieceAsync(Guid kitId, UpdateCraftingKitPieceDto updateDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return null;
        }

        await EnsurePatternBelongsToUserAsync(updateDto.PatternId, userId);
        var piece = await _context.CraftingKitPieces.SingleOrDefaultAsync(piece => piece.Id == updateDto.Id && piece.KitId == kitId);
        if (piece == null)
        {
            return null;
        }

        piece.Update(updateDto.Name, updateDto.PatternId, updateDto.Notes);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();

        return await MapPieceAsync(piece.Id);
    }

    public async Task<bool> DeletePieceAsync(Guid kitId, Guid pieceId, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return false;
        }

        var piece = await _context.CraftingKitPieces.SingleOrDefaultAsync(piece => piece.Id == pieceId && piece.KitId == kitId);
        if (piece == null)
        {
            return false;
        }

        await ClearProjectKitPieceLinksAsync(pieceId);
        _context.CraftingKitPieces.Remove(piece);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();
        await NormalizePieceOrderAsync(kitId);
        return true;
    }

    public async Task<bool> ReorderPiecesAsync(Guid kitId, ReorderCraftingKitItemsDto reorderDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return false;
        }

        var pieces = await _context.CraftingKitPieces.Where(piece => piece.KitId == kitId).ToListAsync();
        if (!ContainsSameIds(pieces.Select(piece => piece.Id), reorderDto.OrderedIds))
        {
            return false;
        }

        for (var i = 0; i < reorderDto.OrderedIds.Count; i++)
        {
            pieces.Single(piece => piece.Id == reorderDto.OrderedIds[i]).MoveTo(i + 1);
        }

        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CraftingProjectDto?> CreateProjectForPieceAsync(
        Guid kitId,
        Guid pieceId,
        CreateCraftingKitProjectDto createDto,
        Guid userId)
    {
        var kit = await _context.CraftingKits.SingleOrDefaultAsync(kit => kit.Id == kitId && kit.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        var piece = await _context.CraftingKitPieces.SingleOrDefaultAsync(piece => piece.Id == pieceId && piece.KitId == kitId);
        if (piece == null)
        {
            return null;
        }

        if (await _context.CraftingProjects.AnyAsync(project => project.KitPieceId == pieceId && project.UserId == userId))
        {
            throw new InvalidOperationException("This kit piece already has a project.");
        }

        var patternId = createDto.PatternId ?? piece.PatternId;
        await EnsurePatternBelongsToUserAsync(patternId, userId);

        var projectName = string.IsNullOrWhiteSpace(createDto.Name)
            ? $"{kit.Name} - {piece.Name}"
            : createDto.Name.Trim();
        var description = string.IsNullOrWhiteSpace(createDto.Description)
            ? piece.Notes
            : createDto.Description.Trim();
        var project = new CraftingProject(Guid.NewGuid(), projectName, userId);
        project.Update(
            projectName,
            description,
            patternId,
            createDto.ThemeId ?? kit.ThemeId,
            createDto.Difficulty ?? kit.Difficulty,
            0);
        project.LinkToKitPiece(kitId, pieceId);

        _context.CraftingProjects.Add(project);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();

        return await new CraftingProjectService(_context).GetByIdAsync(project.Id, userId);
    }

    public async Task<CraftingKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateCraftingKitSupplyDto createDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return null;
        }

        var nextSortOrder = await _context.CraftingKitSupplies
            .Where(supply => supply.KitId == kitId)
            .Select(supply => (int?)supply.SortOrder)
            .MaxAsync() ?? 0;

        var supply = new CraftingKitSupply(Guid.NewGuid(), kitId, createDto.SupplyType, createDto.Name, createDto.Quantity, createDto.Notes, nextSortOrder + 1);
        _context.CraftingKitSupplies.Add(supply);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();

        return await MapSupplyAsync(supply.Id);
    }

    public async Task<CraftingKitSupplyDto?> UpdateSupplyAsync(Guid kitId, UpdateCraftingKitSupplyDto updateDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return null;
        }

        var supply = await _context.CraftingKitSupplies.SingleOrDefaultAsync(supply => supply.Id == updateDto.Id && supply.KitId == kitId);
        if (supply == null)
        {
            return null;
        }

        supply.Update(updateDto.SupplyType, updateDto.Name, updateDto.Quantity, updateDto.Notes);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();

        return await MapSupplyAsync(supply.Id);
    }

    public async Task<bool> DeleteSupplyAsync(Guid kitId, Guid supplyId, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return false;
        }

        var supply = await _context.CraftingKitSupplies.SingleOrDefaultAsync(supply => supply.Id == supplyId && supply.KitId == kitId);
        if (supply == null)
        {
            return false;
        }

        _context.CraftingKitSupplies.Remove(supply);
        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();
        await NormalizeSupplyOrderAsync(kitId);
        return true;
    }

    public async Task<bool> ReorderSuppliesAsync(Guid kitId, ReorderCraftingKitItemsDto reorderDto, Guid userId)
    {
        if (!await KitExistsAsync(kitId, userId))
        {
            return false;
        }

        var supplies = await _context.CraftingKitSupplies.Where(supply => supply.KitId == kitId).ToListAsync();
        if (!ContainsSameIds(supplies.Select(supply => supply.Id), reorderDto.OrderedIds))
        {
            return false;
        }

        for (var i = 0; i < reorderDto.OrderedIds.Count; i++)
        {
            supplies.Single(supply => supply.Id == reorderDto.OrderedIds[i]).MoveTo(i + 1);
        }

        await TouchKitAsync(kitId);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<CraftingKitDto> MapAsync(CraftingKit kit)
    {
        var themeName = kit.ThemeId == null
            ? string.Empty
            : await _context.Themes
                .Where(theme => theme.Id == kit.ThemeId)
                .Select(theme => theme.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        return new CraftingKitDto
        {
            Id = kit.Id,
            Name = kit.Name,
            Slug = kit.Slug,
            Description = kit.Description,
            Type = kit.Type,
            ThemeId = kit.ThemeId,
            ThemeName = themeName,
            Difficulty = kit.Difficulty,
            DifficultyLabel = GetDifficultyLabel(kit.Difficulty),
            Progress = kit.Progress,
            IsArchived = kit.IsArchived,
            ArchivedAt = kit.ArchivedAt,
            Pieces = await MapPiecesAsync(kit.Id),
            Supplies = await MapSuppliesAsync(kit.Id),
            CreatedAt = kit.CreatedAt,
            UpdatedAt = kit.UpdatedAt
        };
    }

    private async Task<IReadOnlyList<CraftingKitDto>> MapListAsync(IReadOnlyList<CraftingKit> kits)
    {
        var results = new List<CraftingKitDto>(kits.Count);
        foreach (var kit in kits)
        {
            results.Add(await MapAsync(kit));
        }

        return results;
    }

    private async Task<IReadOnlyList<CraftingKitPieceDto>> MapPiecesAsync(Guid kitId)
    {
        var pieces = await _context.CraftingKitPieces
            .Where(piece => piece.KitId == kitId)
            .OrderBy(piece => piece.SortOrder)
            .ToListAsync();

        var patternIds = pieces
            .Where(piece => piece.PatternId != null)
            .Select(piece => piece.PatternId!.Value)
            .ToHashSet();
        var patternNames = await _context.CraftingPatterns
            .Where(pattern => patternIds.Contains(pattern.Id))
            .ToDictionaryAsync(pattern => pattern.Id, pattern => pattern.Name);

        return pieces
            .Select(piece => new CraftingKitPieceDto
            {
                Id = piece.Id,
                KitId = piece.KitId,
                Name = piece.Name,
                PatternId = piece.PatternId,
                PatternName = piece.PatternId != null && patternNames.TryGetValue(piece.PatternId.Value, out var patternName)
                    ? patternName
                    : string.Empty,
                Notes = piece.Notes,
                SortOrder = piece.SortOrder
            })
            .ToList();
    }

    private async Task<CraftingKitPieceDto?> MapPieceAsync(Guid pieceId)
    {
        var piece = await _context.CraftingKitPieces.SingleOrDefaultAsync(piece => piece.Id == pieceId);
        if (piece == null)
        {
            return null;
        }

        var patternName = piece.PatternId == null
            ? string.Empty
            : await _context.CraftingPatterns
                .Where(pattern => pattern.Id == piece.PatternId.Value)
                .Select(pattern => pattern.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        return new CraftingKitPieceDto
        {
            Id = piece.Id,
            KitId = piece.KitId,
            Name = piece.Name,
            PatternId = piece.PatternId,
            PatternName = patternName,
            Notes = piece.Notes,
            SortOrder = piece.SortOrder
        };
    }

    private async Task<IReadOnlyList<CraftingKitSupplyDto>> MapSuppliesAsync(Guid kitId)
    {
        return await _context.CraftingKitSupplies
            .Where(supply => supply.KitId == kitId)
            .OrderBy(supply => supply.SortOrder)
            .Select(supply => new CraftingKitSupplyDto
            {
                Id = supply.Id,
                KitId = supply.KitId,
                SupplyType = supply.SupplyType,
                Name = supply.Name,
                Quantity = supply.Quantity,
                Notes = supply.Notes,
                SortOrder = supply.SortOrder
            })
            .ToListAsync();
    }

    private async Task<CraftingKitSupplyDto?> MapSupplyAsync(Guid supplyId)
    {
        return await _context.CraftingKitSupplies
            .Where(supply => supply.Id == supplyId)
            .Select(supply => new CraftingKitSupplyDto
            {
                Id = supply.Id,
                KitId = supply.KitId,
                SupplyType = supply.SupplyType,
                Name = supply.Name,
                Quantity = supply.Quantity,
                Notes = supply.Notes,
                SortOrder = supply.SortOrder
            })
            .SingleOrDefaultAsync();
    }

    private async Task<bool> KitExistsAsync(Guid kitId, Guid userId)
    {
        return await _context.CraftingKits.AnyAsync(kit => kit.Id == kitId && kit.UserId == userId);
    }

    private async Task EnsurePatternBelongsToUserAsync(Guid? patternId, Guid userId)
    {
        if (patternId == null)
        {
            return;
        }

        var exists = await _context.CraftingPatterns.AnyAsync(pattern => pattern.Id == patternId && pattern.UserId == userId);
        if (!exists)
        {
            throw new InvalidOperationException("The selected pattern is not available for this kit.");
        }
    }

    private async Task TouchKitAsync(Guid kitId)
    {
        var kit = await _context.CraftingKits.SingleAsync(kit => kit.Id == kitId);
        kit.UpdatedAt = DateTime.UtcNow;
    }

    private async Task NormalizePieceOrderAsync(Guid kitId)
    {
        var pieces = await _context.CraftingKitPieces
            .Where(piece => piece.KitId == kitId)
            .OrderBy(piece => piece.SortOrder)
            .ToListAsync();

        for (var i = 0; i < pieces.Count; i++)
        {
            pieces[i].MoveTo(i + 1);
        }

        await _context.SaveChangesAsync();
    }

    private async Task ClearProjectKitLinksAsync(Guid kitId)
    {
        var projects = await _context.CraftingProjects
            .Where(project => project.KitId == kitId)
            .ToListAsync();

        foreach (var project in projects)
        {
            project.KitId = null;
            project.KitPieceId = null;
            project.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task ClearProjectKitPieceLinksAsync(Guid kitPieceId)
    {
        var projects = await _context.CraftingProjects
            .Where(project => project.KitPieceId == kitPieceId)
            .ToListAsync();

        foreach (var project in projects)
        {
            project.KitId = null;
            project.KitPieceId = null;
            project.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task NormalizeSupplyOrderAsync(Guid kitId)
    {
        var supplies = await _context.CraftingKitSupplies
            .Where(supply => supply.KitId == kitId)
            .OrderBy(supply => supply.SortOrder)
            .ToListAsync();

        for (var i = 0; i < supplies.Count; i++)
        {
            supplies[i].MoveTo(i + 1);
        }

        await _context.SaveChangesAsync();
    }

    private static string GetDifficultyLabel(int difficulty)
    {
        return Enum.IsDefined(typeof(Difficulty), difficulty)
            ? ((Difficulty)difficulty).ToString()
            : string.Empty;
    }

    private static string UseIncomingValue(string? incomingValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(incomingValue)
            ? currentValue
            : incomingValue.Trim();
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
