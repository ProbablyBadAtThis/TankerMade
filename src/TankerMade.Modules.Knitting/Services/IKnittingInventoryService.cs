using TankerMade.Modules.Knitting.DTOs.Inventory;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingInventoryService
{
    Task<IReadOnlyList<KnittingSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null);
    Task<KnittingSupplyItemDto> CreateSupplyAsync(CreateKnittingSupplyItemDto createDto, Guid userId);
}
