using TankerMade.Contracts.DTOs.ModuleInventory;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleCraftInventoryCapabilityHandler : IModuleInventoryCapabilityHandler
{
    Task<IReadOnlyList<ModuleYarnInventoryItemDto>> GetYarnsAsync(Guid userId, ModuleYarnInventoryFilterRequest? filter = null);
    Task<ModuleYarnInventoryItemDto> CreateOrMergeYarnAsync(CreateModuleYarnInventoryItemDto request, Guid userId);
    Task<ModuleYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<ModuleToolInventoryItemDto>> GetToolsAsync(Guid userId, ModuleToolInventoryFilterRequest? filter = null);
    Task<ModuleToolInventoryItemDto> CreateOrMergeToolAsync(CreateModuleToolInventoryItemDto request, Guid userId);
    Task<ModuleToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<ModuleNotionInventoryItemDto>> GetNotionsAsync(Guid userId, ModuleNotionInventoryFilterRequest? filter = null);
    Task<ModuleNotionInventoryItemDto> CreateOrMergeNotionAsync(CreateModuleNotionInventoryItemDto request, Guid userId);
    Task<ModuleNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<ModuleInventoryReferenceItemDto>> GetReferenceItemsAsync(string category);
    Task<ModuleInventoryReferenceItemDto> CreateReferenceItemAsync(string category, CreateModuleInventoryReferenceItemDto request);
}
