using TankerMade.Modules.Knitting.DTOs.Patterns;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingPatternService
{
    Task<KnittingPatternDto> CreateAsync(CreateKnittingPatternDto createDto, Guid userId);
    Task<KnittingPatternDto?> GetByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<KnittingPatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IReadOnlyList<KnittingPatternDto>> SearchAsync(string searchTerm, Guid userId, int page = 1, int pageSize = 50);
    Task<KnittingPatternDto?> UpdateAsync(UpdateKnittingPatternDto updateDto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<KnittingPatternPieceDto?> AddPieceAsync(Guid patternId, CreateKnittingPatternPieceDto createDto, Guid userId);
    Task<KnittingPatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateKnittingPatternPieceDto updateDto, Guid userId);
    Task<bool> DeletePieceAsync(Guid patternId, Guid pieceId, Guid userId);
    Task<bool> ReorderPiecesAsync(Guid patternId, ReorderKnittingPatternItemsDto reorderDto, Guid userId);
    Task<KnittingPatternStepDto?> AddStepAsync(Guid patternId, Guid pieceId, CreateKnittingPatternStepDto createDto, Guid userId);
    Task<KnittingPatternStepDto?> UpdateStepAsync(Guid patternId, Guid pieceId, UpdateKnittingPatternStepDto updateDto, Guid userId);
    Task<bool> DeleteStepAsync(Guid patternId, Guid pieceId, Guid stepId, Guid userId);
    Task<bool> ReorderStepsAsync(Guid patternId, Guid pieceId, ReorderKnittingPatternItemsDto reorderDto, Guid userId);
}
