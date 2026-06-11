using TankerMade.Contracts.DTOs.ModuleKits;
using TankerMade.Contracts.DTOs.ModuleProjects;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Kits;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingKitCapabilityHandler : IModuleKitCapabilityHandler
{
    private readonly IKnittingKitService _service;

    public KnittingKitCapabilityHandler(IKnittingKitService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleKitDto>> GetAllAsync(Guid userId, bool includeArchived = false)
        => (await _service.GetAllAsync(userId, includeArchived)).Select(Map).ToList();

    public async Task<ModuleKitDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var result = await _service.GetByIdAsync(id, userId);
        return result == null ? null : Map(result);
    }

    public async Task<ModuleKitDto> CreateAsync(CreateModuleKitRequest request, Guid userId)
        => Map(await _service.CreateAsync(new CreateKnittingKitDto
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type
        }, userId));

    public async Task<ModuleKitDto?> UpdateAsync(Guid kitId, UpdateModuleKitRequest request, Guid userId)
    {
        var kit = await _service.UpdateAsync(kitId, new CreateKnittingKitDto
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type
        }, userId);

        return kit == null ? null : Map(kit);
    }

    public Task<bool> DeleteAsync(Guid kitId, Guid userId)
        => _service.DeleteAsync(kitId, userId);

    public async Task<ModuleKitDto?> ArchiveAsync(Guid kitId, Guid userId)
    {
        var kit = await _service.ArchiveAsync(kitId, userId);
        return kit == null ? null : Map(kit);
    }

    public async Task<ModuleKitDto?> ReopenAsync(Guid kitId, Guid userId)
    {
        var kit = await _service.ReopenAsync(kitId, userId);
        return kit == null ? null : Map(kit);
    }

    public async Task<ModuleKitPieceDto?> AddPieceAsync(Guid kitId, CreateModuleKitPieceRequest request, Guid userId)
    {
        var piece = await _service.AddPieceAsync(kitId, new CreateKnittingKitPieceDto { Name = request.Name, Notes = request.Notes }, userId);
        return piece == null ? null : MapPiece(piece);
    }

    public async Task<ModuleKitPieceDto?> UpdatePieceAsync(Guid kitId, Guid pieceId, UpdateModuleKitPieceRequest request, Guid userId)
    {
        var piece = await _service.UpdatePieceAsync(kitId, pieceId, new CreateKnittingKitPieceDto
        {
            Name = request.Name,
            Notes = request.Notes
        }, userId);

        return piece == null ? null : MapPiece(piece);
    }

    public Task<bool> DeletePieceAsync(Guid kitId, Guid pieceId, Guid userId)
        => _service.DeletePieceAsync(kitId, pieceId, userId);

    public async Task<ModuleKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateModuleKitSupplyRequest request, Guid userId)
    {
        var supply = await _service.AddSupplyAsync(kitId, new CreateKnittingKitSupplyDto
        {
            SupplyType = request.SupplyType,
            Name = request.Name,
            InventoryItemId = request.InventoryItemId,
            Quantity = request.Quantity
        }, userId);
        return supply == null ? null : MapSupply(supply);
    }

    public async Task<ModuleKitSupplyDto?> UpdateSupplyAsync(Guid kitId, Guid supplyId, UpdateModuleKitSupplyRequest request, Guid userId)
    {
        var supply = await _service.UpdateSupplyAsync(kitId, supplyId, new CreateKnittingKitSupplyDto
        {
            SupplyType = request.SupplyType,
            Name = request.Name,
            InventoryItemId = request.InventoryItemId,
            Quantity = request.Quantity
        }, userId);

        return supply == null ? null : MapSupply(supply);
    }

    public Task<bool> DeleteSupplyAsync(Guid kitId, Guid supplyId, Guid userId)
        => _service.DeleteSupplyAsync(kitId, supplyId, userId);

    public async Task<ModuleProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, Guid userId)
    {
        var project = await _service.CreateProjectForPieceAsync(kitId, pieceId, userId);
        return project == null ? null : MapProject(project);
    }

    private static ModuleKitDto Map(KnittingKitDto source)
    {
        return new ModuleKitDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Type = source.Type,
            IsArchived = source.IsArchived,
            ArchivedAt = source.ArchivedAt,
            Pieces = source.Pieces.Select(MapPiece).ToList(),
            Supplies = source.Supplies.Select(MapSupply).ToList(),
            UserId = source.UserId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    private static ModuleKitPieceDto MapPiece(KnittingKitPieceDto piece)
    {
        return new ModuleKitPieceDto
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

    private static ModuleKitSupplyDto MapSupply(KnittingKitSupplyDto supply)
    {
        return new ModuleKitSupplyDto
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

    private static ModuleProjectDto MapProject(KnittingProjectDto source)
    {
        return new ModuleProjectDto
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            Description = source.Description,
            PatternId = source.PatternId,
            PatternName = source.PatternName,
            ThemeId = source.ThemeId,
            ThemeName = source.ThemeName,
            Difficulty = source.Difficulty,
            Progress = source.Progress,
            IsArchived = source.IsArchived,
            ArchivedAt = source.ArchivedAt,
            UserId = source.UserId,
            Username = source.Username,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
