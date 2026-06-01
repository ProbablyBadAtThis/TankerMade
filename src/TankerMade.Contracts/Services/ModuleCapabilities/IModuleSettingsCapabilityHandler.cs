using TankerMade.Contracts.DTOs.ModuleSettings;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleSettingsCapabilityHandler
{
    string ModuleKey { get; }

    Task<IReadOnlyList<ModuleSettingItemDto>> GetAllAsync(Guid userId, string? category = null);
    Task<ModuleSettingItemDto> UpsertAsync(UpsertModuleSettingItemRequest request, Guid userId);
}
