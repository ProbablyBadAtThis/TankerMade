using TankerMade.Modules.Crafting.DTOs.Projects;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingProjectService
{
    Task<CraftingProjectDto> CreateAsync(CreateCraftingProjectDto createDto, Guid userId);
    Task<CraftingProjectDto?> GetByIdAsync(Guid id, Guid userId);
    Task<CraftingProjectDto?> GetBySlugAsync(string slug, Guid userId);
    Task<IReadOnlyList<CraftingProjectDto>> GetAllAsync(Guid userId);
    Task<CraftingProjectDto?> UpdateAsync(UpdateCraftingProjectDto updateDto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingProjectDto>> SearchAsync(string searchTerm, Guid userId);
    Task<CraftingProjectDto?> SetStepProgressAsync(Guid projectId, Guid patternStepId, UpdateCraftingProjectStepProgressDto updateDto, Guid userId);
}
