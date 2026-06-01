using TankerMade.Contracts.DTOs.ModuleKits;
using TankerMade.Contracts.DTOs.ModuleProjects;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleKitCapabilityHandler
{
    string ModuleKey { get; }

    Task<IReadOnlyList<ModuleKitDto>> GetAllAsync(Guid userId, bool includeArchived = false);
    Task<ModuleKitDto?> GetByIdAsync(Guid id, Guid userId);
    Task<ModuleKitDto> CreateAsync(CreateModuleKitRequest request, Guid userId);
    Task<ModuleKitPieceDto?> AddPieceAsync(Guid kitId, CreateModuleKitPieceRequest request, Guid userId);
    Task<ModuleKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateModuleKitSupplyRequest request, Guid userId);
    Task<ModuleProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, Guid userId);
}
