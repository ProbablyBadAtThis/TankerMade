using TankerMade.Modules.Printing3D.DTOs.Inventory;

namespace TankerMade.Modules.Printing3D.Services;

public interface IPrintingInventoryService
{
    Task<PrintingMaterialInventoryItemDto> CreateOrMergeMaterialAsync(
        CreatePrintingMaterialInventoryItemDto createDto,
        Guid userId);

    Task<PrintingMaterialInventoryItemDto?> GetMaterialByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<PrintingMaterialInventoryItemDto>> GetMaterialsAsync(
        Guid userId,
        PrintingMaterialInventoryFilterDto? filter = null);
    Task<IReadOnlyList<PrintingInventoryReferenceItemDto>> GetReferenceItemsAsync(string category);
    Task<PrintingInventoryReferenceItemDto> CreateReferenceItemAsync(string category, CreatePrintingInventoryReferenceItemDto request);
}
