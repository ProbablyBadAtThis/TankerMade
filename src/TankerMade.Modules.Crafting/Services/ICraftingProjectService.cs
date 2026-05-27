using TankerMade.Modules.Crafting.DTOs.Projects;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingProjectService
{
    Task<CraftingProjectDto> CreateAsync(CreateCraftingProjectDto createDto, Guid userId);
    Task<CraftingProjectDto?> GetByIdAsync(Guid id, Guid userId);
    Task<CraftingProjectDto?> GetBySlugAsync(string slug, Guid userId);
    Task<IReadOnlyList<CraftingProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false);
    Task<CraftingProjectDto?> UpdateAsync(UpdateCraftingProjectDto updateDto, Guid userId);
    Task<CraftingProjectDto?> ArchiveAsync(Guid id, Guid userId);
    Task<CraftingProjectDto?> ReopenAsync(Guid id, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingProjectDto>> SearchAsync(string searchTerm, Guid userId);
    Task<CraftingProjectDto?> SetStepProgressAsync(Guid projectId, Guid patternStepId, UpdateCraftingProjectStepProgressDto updateDto, Guid userId);
    Task<CraftingProjectDto?> StartTimerAsync(Guid projectId, Guid patternStepId, UpdateCraftingProjectTimerDto updateDto, Guid userId);
    Task<CraftingProjectDto?> PauseTimerAsync(Guid projectId, Guid patternStepId, UpdateCraftingProjectTimerDto updateDto, Guid userId);
    Task<CraftingProjectDto?> SetTimerAsync(Guid projectId, Guid patternStepId, UpdateCraftingProjectTimerDto updateDto, Guid userId);
    Task<CraftingProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId);
    Task<CraftingProjectDto?> AddInventoryLinkAsync(Guid projectId, CreateCraftingProjectInventoryLinkDto createDto, Guid userId);
    Task<bool> RemoveInventoryLinkAsync(Guid projectId, Guid linkId, Guid userId);
}
