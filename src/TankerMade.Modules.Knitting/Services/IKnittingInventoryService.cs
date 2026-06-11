using TankerMade.Modules.Knitting.DTOs.Inventory;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingInventoryService
{
    Task<KnittingYarnInventoryItemDto> CreateOrMergeYarnAsync(CreateKnittingYarnInventoryItemDto createDto, Guid userId);
    Task<KnittingYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<KnittingYarnInventoryItemDto>> GetYarnsAsync(Guid userId, KnittingYarnInventoryFilterDto? filter = null);
    Task<KnittingToolInventoryItemDto> CreateOrMergeToolAsync(CreateKnittingToolInventoryItemDto createDto, Guid userId);
    Task<KnittingToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<KnittingToolInventoryItemDto>> GetToolsAsync(Guid userId, KnittingToolInventoryFilterDto? filter = null);
    Task<KnittingNotionInventoryItemDto> CreateOrMergeNotionAsync(CreateKnittingNotionInventoryItemDto createDto, Guid userId);
    Task<KnittingNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<KnittingNotionInventoryItemDto>> GetNotionsAsync(Guid userId, KnittingNotionInventoryFilterDto? filter = null);
    Task<IReadOnlyList<KnittingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category);
    Task<KnittingInventoryReferenceItemDto> CreateReferenceItemAsync(string category, CreateKnittingInventoryReferenceItemDto request);

    // Legacy flat supply compatibility for neutral capability endpoint.
    Task<IReadOnlyList<KnittingSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null);
    Task<KnittingSupplyItemDto> CreateSupplyAsync(CreateKnittingSupplyItemDto createDto, Guid userId);
    Task<KnittingSupplyItemDto?> UpdateSupplyAsync(Guid supplyId, CreateKnittingSupplyItemDto updateDto, Guid userId);
    Task<bool> DeleteSupplyAsync(Guid supplyId, Guid userId);
}
