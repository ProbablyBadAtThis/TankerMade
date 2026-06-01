using TankerMade.Contracts.DTOs.ModuleInventory;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleInventoryCapabilityHandler
{
    string ModuleKey { get; }

    Task<IReadOnlyList<ModuleSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null);
    Task<ModuleSupplyItemDto> CreateSupplyAsync(CreateModuleSupplyItemRequest request, Guid userId);
}
