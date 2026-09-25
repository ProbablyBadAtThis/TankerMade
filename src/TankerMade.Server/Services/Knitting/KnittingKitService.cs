using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Knitting.DTOs.Kits;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Knitting;

public class KnittingKitService : IKnittingKitService
{
    private readonly TankerMadeDbContext _context;

    public KnittingKitService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KnittingKitDto>> GetAllAsync(Guid userId, bool includeArchived = false)
    {
        var kits = await _context.KnittingKits
            .Where(kit => kit.UserId == userId && (includeArchived || !kit.IsArchived))
            .OrderBy(kit => kit.IsArchived)
            .ThenBy(kit => kit.Name)
            .ToListAsync();

        var result = new List<KnittingKitDto>(kits.Count);
        foreach (var kit in kits)
        {
            result.Add(await MapAsync(kit));
        }

        return result;
    }

    public async Task<KnittingKitDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(kit => kit.Id == id && kit.UserId == userId);
        return kit == null ? null : await MapAsync(kit);
    }

    public async Task<KnittingKitDto> CreateAsync(CreateKnittingKitDto createDto, Guid userId)
    {
        var kit = new KnittingKit(Guid.NewGuid(), userId, createDto.Name);
        kit.Update(createDto.Description, createDto.Type);

        _context.KnittingKits.Add(kit);
        await _context.SaveChangesAsync();

        return await MapAsync(kit);
    }

    public async Task<KnittingKitDto?> UpdateAsync(Guid kitId, CreateKnittingKitDto updateDto, Guid userId)
    {
        var kit = await _context.KnittingKits
            .SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Name))
        {
            kit.Name = updateDto.Name.Trim();
        }

        kit.Update(updateDto.Description, updateDto.Type);
        await _context.SaveChangesAsync();

        return await MapAsync(kit);
    }

    public async Task<bool> DeleteAsync(Guid kitId, Guid userId)
    {
        var kit = await _context.KnittingKits
            .SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return false;
        }

        _context.KnittingKits.Remove(kit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<KnittingKitDto?> ArchiveAsync(Guid kitId, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        kit.Archive();
        await _context.SaveChangesAsync();
        return await GetByIdAsync(kitId, userId);
    }

    public async Task<KnittingKitDto?> ReopenAsync(Guid kitId, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        kit.Reopen();
        await _context.SaveChangesAsync();
        return await GetByIdAsync(kitId, userId);
    }

    public async Task<KnittingKitPieceDto?> AddPieceAsync(Guid kitId, CreateKnittingKitPieceDto createDto, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        var nextSort = await _context.KnittingKitPieces
            .Where(piece => piece.KitId == kitId)
            .Select(piece => (int?)piece.SortOrder)
            .MaxAsync() ?? 0;

        var piece = new KnittingKitPiece(Guid.NewGuid(), kitId, createDto.Name, nextSort + 1);
        piece.Update(createDto.Notes);

        _context.KnittingKitPieces.Add(piece);
        await _context.SaveChangesAsync();

        return MapPiece(piece);
    }

    public async Task<KnittingKitPieceDto?> UpdatePieceAsync(Guid kitId, Guid pieceId, CreateKnittingKitPieceDto updateDto, Guid userId)
    {
        var kitExists = await _context.KnittingKits.AnyAsync(kit => kit.Id == kitId && kit.UserId == userId);
        if (!kitExists)
        {
            return null;
        }

        var piece = await _context.KnittingKitPieces
            .SingleOrDefaultAsync(existing => existing.Id == pieceId && existing.KitId == kitId);
        if (piece == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Name))
        {
            piece.Name = updateDto.Name.Trim();
        }

        piece.Update(updateDto.Notes);
        await _context.SaveChangesAsync();

        return MapPiece(piece);
    }

    public async Task<bool> DeletePieceAsync(Guid kitId, Guid pieceId, Guid userId)
    {
        var kitExists = await _context.KnittingKits.AnyAsync(kit => kit.Id == kitId && kit.UserId == userId);
        if (!kitExists)
        {
            return false;
        }

        var piece = await _context.KnittingKitPieces
            .SingleOrDefaultAsync(existing => existing.Id == pieceId && existing.KitId == kitId);
        if (piece == null)
        {
            return false;
        }

        _context.KnittingKitPieces.Remove(piece);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<KnittingKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateKnittingKitSupplyDto createDto, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        var nextSort = await _context.KnittingKitSupplies
            .Where(supply => supply.KitId == kitId)
            .Select(supply => (int?)supply.SortOrder)
            .MaxAsync() ?? 0;

        var supply = new KnittingKitSupply(Guid.NewGuid(), kitId, createDto.SupplyType, createDto.Name, nextSort + 1);
        supply.Update(createDto.SupplyType, createDto.Name, createDto.InventoryItemId, createDto.Quantity);

        _context.KnittingKitSupplies.Add(supply);
        await _context.SaveChangesAsync();

        return MapSupply(supply);
    }

    public async Task<KnittingKitSupplyDto?> UpdateSupplyAsync(Guid kitId, Guid supplyId, CreateKnittingKitSupplyDto updateDto, Guid userId)
    {
        var kitExists = await _context.KnittingKits.AnyAsync(kit => kit.Id == kitId && kit.UserId == userId);
        if (!kitExists)
        {
            return null;
        }

        var supply = await _context.KnittingKitSupplies
            .SingleOrDefaultAsync(existing => existing.Id == supplyId && existing.KitId == kitId);
        if (supply == null)
        {
            return null;
        }

        supply.Update(
            string.IsNullOrWhiteSpace(updateDto.SupplyType) ? supply.SupplyType : updateDto.SupplyType,
            string.IsNullOrWhiteSpace(updateDto.Name) ? supply.Name : updateDto.Name,
            updateDto.InventoryItemId ?? supply.InventoryItemId,
            updateDto.Quantity);
        await _context.SaveChangesAsync();

        return MapSupply(supply);
    }

    public async Task<bool> DeleteSupplyAsync(Guid kitId, Guid supplyId, Guid userId)
    {
        var kitExists = await _context.KnittingKits.AnyAsync(kit => kit.Id == kitId && kit.UserId == userId);
        if (!kitExists)
        {
            return false;
        }

        var supply = await _context.KnittingKitSupplies
            .SingleOrDefaultAsync(existing => existing.Id == supplyId && existing.KitId == kitId);
        if (supply == null)
        {
            return false;
        }

        _context.KnittingKitSupplies.Remove(supply);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<KnittingProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, Guid userId)
    {
        var kit = await _context.KnittingKits.SingleOrDefaultAsync(existing => existing.Id == kitId && existing.UserId == userId);
        if (kit == null)
        {
            return null;
        }

        var piece = await _context.KnittingKitPieces.SingleOrDefaultAsync(existing => existing.Id == pieceId && existing.KitId == kitId);
        if (piece == null)
        {
            return null;
        }

        if (piece.ProjectId.HasValue)
        {
            var existing = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == piece.ProjectId.Value && project.UserId == userId);
            return existing == null ? null : MapProject(existing, string.Empty, string.Empty, userId);
        }

        var project = new KnittingProject(Guid.NewGuid(), piece.Name, userId);
        project.Update(piece.Name, piece.Notes, null, null, null, 0, 0);

        _context.KnittingProjects.Add(project);
        await _context.SaveChangesAsync();

        piece.LinkProject(project.Id);
        await _context.SaveChangesAsync();

        return MapProject(project, string.Empty, string.Empty, userId);
    }

    private async Task<KnittingKitDto> MapAsync(KnittingKit kit)
    {
        var pieces = await _context.KnittingKitPieces
            .Where(piece => piece.KitId == kit.Id)
            .OrderBy(piece => piece.SortOrder)
            .ToListAsync();

        var supplies = await _context.KnittingKitSupplies
            .Where(supply => supply.KitId == kit.Id)
            .OrderBy(supply => supply.SortOrder)
            .ToListAsync();

        return new KnittingKitDto
        {
            Id = kit.Id,
            Name = kit.Name,
            Description = kit.Description,
            Type = kit.Type,
            IsArchived = kit.IsArchived,
            ArchivedAt = kit.ArchivedAt,
            Pieces = pieces.Select(MapPiece).ToList(),
            Supplies = supplies.Select(MapSupply).ToList(),
            UserId = kit.UserId,
            CreatedAt = kit.CreatedAt,
            UpdatedAt = kit.UpdatedAt
        };
    }

    private static KnittingKitPieceDto MapPiece(KnittingKitPiece piece)
    {
        return new KnittingKitPieceDto
        {
            Id = piece.Id,
            Name = piece.Name,
            Notes = piece.Notes,
            SortOrder = piece.SortOrder,
            ProjectId = piece.ProjectId,
            CreatedAt = piece.CreatedAt,
            UpdatedAt = piece.UpdatedAt
        };
    }

    private static KnittingKitSupplyDto MapSupply(KnittingKitSupply supply)
    {
        return new KnittingKitSupplyDto
        {
            Id = supply.Id,
            SupplyType = supply.SupplyType,
            Name = supply.Name,
            InventoryItemId = supply.InventoryItemId,
            Quantity = supply.Quantity,
            SortOrder = supply.SortOrder,
            CreatedAt = supply.CreatedAt,
            UpdatedAt = supply.UpdatedAt
        };
    }

    private static KnittingProjectDto MapProject(KnittingProject project, string patternName, string themeName, Guid userId)
    {
        return new KnittingProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            Description = project.Description,
            PatternId = project.PatternId,
            PatternName = patternName,
            ThemeId = project.ThemeId,
            ThemeName = themeName,
            Difficulty = project.Difficulty,
            Progress = project.Progress,
            IsArchived = project.IsArchived,
            ArchivedAt = project.ArchivedAt,
            UserId = userId,
            Username = string.Empty,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
