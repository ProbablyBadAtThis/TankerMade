using TankerMade.Modules.Crafting.DTOs.Inventory;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingInventoryService
{
    Task<CraftingYarnInventoryItemDto> CreateOrMergeYarnAsync(CreateCraftingYarnInventoryItemDto createDto, Guid userId);
    Task<CraftingYarnInventoryItemDto?> GetYarnByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingYarnInventoryItemDto>> GetYarnsAsync(Guid userId, CraftingYarnInventoryFilterDto? filter = null);
    Task<CraftingToolInventoryItemDto> CreateOrMergeToolAsync(CreateCraftingToolInventoryItemDto createDto, Guid userId);
    Task<CraftingToolInventoryItemDto?> GetToolByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingToolInventoryItemDto>> GetToolsAsync(Guid userId, CraftingToolInventoryFilterDto? filter = null);
    Task<CraftingNotionInventoryItemDto> CreateOrMergeNotionAsync(CreateCraftingNotionInventoryItemDto createDto, Guid userId);
    Task<CraftingNotionInventoryItemDto?> GetNotionByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingNotionInventoryItemDto>> GetNotionsAsync(Guid userId, CraftingNotionInventoryFilterDto? filter = null);
    Task<IReadOnlyList<CraftingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category);
    Task<CraftingInventoryReferenceItemDto> CreateReferenceItemAsync(string category, CreateCraftingInventoryReferenceItemDto request);
}
