using TankerMade.Modules.Crafting.DTOs.Patterns;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingPatternService
{
    Task<CraftingPatternDto> CreateAsync(CreateCraftingPatternDto createDto, Guid userId);
    Task<CraftingPatternDto?> GetByIdAsync(Guid id, Guid userId);
    Task<CraftingPatternDto?> GetBySlugAsync(string slug, Guid userId);
    Task<IReadOnlyList<CraftingPatternDto>> GetAllAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IReadOnlyList<CraftingPatternDto>> GetByThemeIdAsync(Guid themeId, Guid userId);
    Task<IReadOnlyList<CraftingPatternDto>> SearchAsync(string searchTerm, Guid userId, int page = 1, int pageSize = 50);
    Task<CraftingPatternDto?> UpdateAsync(UpdateCraftingPatternDto updateDto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<CraftingPatternPieceDto?> AddPieceAsync(Guid patternId, CreateCraftingPatternPieceDto createDto, Guid userId);
    Task<CraftingPatternPieceDto?> UpdatePieceAsync(Guid patternId, UpdateCraftingPatternPieceDto updateDto, Guid userId);
    Task<bool> DeletePieceAsync(Guid patternId, Guid pieceId, Guid userId);
    Task<bool> ReorderPiecesAsync(Guid patternId, ReorderCraftingPatternItemsDto reorderDto, Guid userId);
    Task<CraftingPatternStepDto?> AddStepAsync(Guid patternId, Guid pieceId, CreateCraftingPatternStepDto createDto, Guid userId);
    Task<CraftingPatternStepDto?> UpdateStepAsync(Guid patternId, Guid pieceId, UpdateCraftingPatternStepDto updateDto, Guid userId);
    Task<bool> DeleteStepAsync(Guid patternId, Guid pieceId, Guid stepId, Guid userId);
    Task<bool> ReorderStepsAsync(Guid patternId, Guid pieceId, ReorderCraftingPatternItemsDto reorderDto, Guid userId);
}
