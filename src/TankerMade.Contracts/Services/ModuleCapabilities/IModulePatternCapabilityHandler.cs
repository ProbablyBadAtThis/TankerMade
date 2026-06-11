using TankerMade.Contracts.DTOs.ModulePatterns;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModulePatternCapabilityHandler
{
    string ModuleKey { get; }

    Task<IReadOnlyList<ModulePatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IReadOnlyList<ModulePatternDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = 50);
    Task<ModulePatternDto?> GetByIdAsync(Guid id, Guid userId);

    Task<ModulePatternDto> CreateAsync(CreateModulePatternRequest request, Guid userId);
    Task<ModulePatternDto?> UpdateAsync(UpdateModulePatternRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);

    Task<ModulePatternPieceDto?> AddPieceAsync(Guid patternId, CreateModulePatternPieceRequest request, Guid userId);
    Task<ModulePatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateModulePatternPieceRequest request, Guid userId);
    Task<bool> DeletePieceAsync(Guid patternId, Guid pieceId, Guid userId);
    Task<bool> ReorderPiecesAsync(Guid patternId, ReorderModulePatternItemsRequest request, Guid userId);

    Task<ModulePatternStepDto?> AddStepAsync(Guid patternId, Guid pieceId, CreateModulePatternStepRequest request, Guid userId);
    Task<ModulePatternStepDto?> UpdateStepAsync(Guid patternId, Guid pieceId, UpdateModulePatternStepRequest request, Guid userId);
    Task<bool> DeleteStepAsync(Guid patternId, Guid pieceId, Guid stepId, Guid userId);
    Task<bool> ReorderStepsAsync(Guid patternId, Guid pieceId, ReorderModulePatternItemsRequest request, Guid userId);

    Task<ModulePatternSupplyDto?> AddSupplyAsync(Guid patternId, CreateModulePatternSupplyRequest request, Guid userId);
    Task<bool> DeleteSupplyAsync(Guid patternId, Guid supplyId, Guid userId);
}
