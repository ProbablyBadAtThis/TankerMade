using TankerMade.Contracts.DTOs.Modules;

namespace TankerMade.Contracts.Services;

public interface IModuleService
{
    Task<IReadOnlyList<ModuleDto>> GetAvailableModulesAsync(Guid userId);
    Task<IReadOnlyList<ModuleDto>> GetActiveModulesAsync(Guid userId);
    Task<ModuleDto?> ActivateAsync(string moduleKey, Guid userId);
    Task<bool> DeactivateAsync(string moduleKey, Guid userId);
    Task<bool> IsActiveAsync(string moduleKey, Guid userId);
}
