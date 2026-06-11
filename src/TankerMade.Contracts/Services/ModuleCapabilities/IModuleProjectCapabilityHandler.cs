using TankerMade.Contracts.DTOs.ModuleProjects;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleProjectCapabilityHandler
{
    string ModuleKey { get; }

    Task<IReadOnlyList<ModuleProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false, int page = 1, int pageSize = 50);
    Task<ModuleProjectDto?> GetByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<ModuleProjectDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = 50);

    Task<ModuleProjectDto> CreateAsync(CreateModuleProjectRequest request, Guid userId);
    Task<ModuleProjectDto?> UpdateAsync(UpdateModuleProjectRequest request, Guid userId);
    Task<ModuleProjectDto?> ArchiveAsync(Guid id, Guid userId);
    Task<ModuleProjectDto?> ReopenAsync(Guid id, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<ModuleProjectDto?> SetStepProgressAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectStepProgressRequest request, Guid userId);
    Task<ModuleProjectDto?> StartTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId);
    Task<ModuleProjectDto?> PauseTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId);
    Task<ModuleProjectDto?> SetTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId);
    Task<ModuleProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId);
    Task<ModuleProjectDto?> AddInventoryLinkAsync(Guid projectId, CreateModuleProjectInventoryLinkRequest request, Guid userId);
    Task<bool> RemoveInventoryLinkAsync(Guid projectId, Guid linkId, Guid userId);
}
