using TankerMade.Modules.Crafting.DTOs.Patterns;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingPatternService
{
    Task<CraftingPatternDto> CreateAsync(CreateCraftingPatternDto createDto, Guid userId);
    Task<CraftingPatternDto?> GetByIdAsync(Guid id, Guid userId);
    Task<CraftingPatternDto?> GetBySlugAsync(string slug, Guid userId);
    Task<IReadOnlyList<CraftingPatternDto>> GetAllAsync(Guid userId);
    Task<IReadOnlyList<CraftingPatternDto>> GetByThemeIdAsync(Guid themeId, Guid userId);
    Task<IReadOnlyList<CraftingPatternDto>> SearchAsync(string searchTerm, Guid userId);
    Task<CraftingPatternDto?> UpdateAsync(UpdateCraftingPatternDto updateDto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
