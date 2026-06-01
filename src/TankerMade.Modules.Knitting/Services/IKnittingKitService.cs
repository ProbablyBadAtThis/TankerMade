using TankerMade.Modules.Knitting.DTOs.Kits;
using TankerMade.Modules.Knitting.DTOs.Projects;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingKitService
{
    Task<IReadOnlyList<KnittingKitDto>> GetAllAsync(Guid userId, bool includeArchived = false);
    Task<KnittingKitDto?> GetByIdAsync(Guid id, Guid userId);
    Task<KnittingKitDto> CreateAsync(CreateKnittingKitDto createDto, Guid userId);
    Task<KnittingKitPieceDto?> AddPieceAsync(Guid kitId, CreateKnittingKitPieceDto createDto, Guid userId);
    Task<KnittingKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateKnittingKitSupplyDto createDto, Guid userId);
    Task<KnittingProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, Guid userId);
}
