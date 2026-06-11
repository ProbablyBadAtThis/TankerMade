using TankerMade.Modules.Knitting.DTOs.Kits;
using TankerMade.Modules.Knitting.DTOs.Projects;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingKitService
{
    Task<IReadOnlyList<KnittingKitDto>> GetAllAsync(Guid userId, bool includeArchived = false);
    Task<KnittingKitDto?> GetByIdAsync(Guid id, Guid userId);
    Task<KnittingKitDto> CreateAsync(CreateKnittingKitDto createDto, Guid userId);
    Task<KnittingKitDto?> UpdateAsync(Guid kitId, CreateKnittingKitDto updateDto, Guid userId);
    Task<bool> DeleteAsync(Guid kitId, Guid userId);
    Task<KnittingKitDto?> ArchiveAsync(Guid kitId, Guid userId);
    Task<KnittingKitDto?> ReopenAsync(Guid kitId, Guid userId);
    Task<KnittingKitPieceDto?> AddPieceAsync(Guid kitId, CreateKnittingKitPieceDto createDto, Guid userId);
    Task<KnittingKitPieceDto?> UpdatePieceAsync(Guid kitId, Guid pieceId, CreateKnittingKitPieceDto updateDto, Guid userId);
    Task<bool> DeletePieceAsync(Guid kitId, Guid pieceId, Guid userId);
    Task<KnittingKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateKnittingKitSupplyDto createDto, Guid userId);
    Task<KnittingKitSupplyDto?> UpdateSupplyAsync(Guid kitId, Guid supplyId, CreateKnittingKitSupplyDto updateDto, Guid userId);
    Task<bool> DeleteSupplyAsync(Guid kitId, Guid supplyId, Guid userId);
    Task<KnittingProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, Guid userId);
}
