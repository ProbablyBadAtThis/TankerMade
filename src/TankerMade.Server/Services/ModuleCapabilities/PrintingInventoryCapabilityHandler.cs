using TankerMade.Contracts.DTOs.ModuleInventory;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Printing3D;
using TankerMade.Modules.Printing3D.DTOs.Inventory;
using TankerMade.Modules.Printing3D.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class PrintingInventoryCapabilityHandler : IModuleInventoryCapabilityHandler
{
    private readonly IPrintingInventoryService _service;

    public PrintingInventoryCapabilityHandler(IPrintingInventoryService service)
    {
        _service = service;
    }

    public string ModuleKey => Printing3DModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null)
    {
        var filter = new PrintingMaterialInventoryFilterDto
        {
            Search = search ?? string.Empty,
            MaterialType = category ?? string.Empty
        };

        var materials = await _service.GetMaterialsAsync(userId, filter);
        return materials.Select(Map).ToList();
    }

    public async Task<ModuleSupplyItemDto> CreateSupplyAsync(CreateModuleSupplyItemRequest request, Guid userId)
    {
        var material = await _service.CreateOrMergeMaterialAsync(new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = string.IsNullOrWhiteSpace(request.Category) ? "Material" : request.Category,
            BrandName = string.IsNullOrWhiteSpace(request.Brand) ? "Unspecified" : request.Brand,
            ColorName = string.IsNullOrWhiteSpace(request.Color) ? request.Name : request.Color,
            SpoolWeightGrams = request.Quantity <= 0 ? 1000 : request.Quantity,
            RemainingWeightGrams = request.Quantity <= 0 ? 1000 : request.Quantity,
            Diameter = request.Size,
            StorageLocation = request.Notes,
            SpoolCode = string.Empty,
            PrinterCompatibility = string.Empty,
            SourceName = string.Empty,
            Price = null,
            IsSalePrice = false,
            PurchasedAt = null
        }, userId);

        return Map(material);
    }

    public async Task<ModuleSupplyItemDto?> UpdateSupplyAsync(Guid supplyId, UpdateModuleSupplyItemRequest request, Guid userId)
    {
        var material = await _service.UpdateMaterialAsync(supplyId, new CreatePrintingMaterialInventoryItemDto
        {
            MaterialType = string.IsNullOrWhiteSpace(request.Category) ? "Material" : request.Category,
            BrandName = string.IsNullOrWhiteSpace(request.Brand) ? "Unspecified" : request.Brand,
            ColorName = string.IsNullOrWhiteSpace(request.Color) ? request.Name : request.Color,
            SpoolWeightGrams = request.Quantity <= 0 ? 0 : request.Quantity,
            RemainingWeightGrams = request.Quantity <= 0 ? null : request.Quantity,
            Diameter = request.Size,
            StorageLocation = request.Notes,
            SpoolCode = string.Empty,
            PrinterCompatibility = string.Empty,
            SourceName = string.Empty,
            Price = null,
            IsSalePrice = false,
            PurchasedAt = null
        }, userId);

        return material == null ? null : Map(material);
    }

    public Task<bool> DeleteSupplyAsync(Guid supplyId, Guid userId)
        => _service.DeleteMaterialAsync(supplyId, userId);

    private static ModuleSupplyItemDto Map(PrintingMaterialInventoryItemDto item)
    {
        return new ModuleSupplyItemDto
        {
            Id = item.Id,
            Category = item.MaterialType,
            Name = item.ColorName,
            Brand = item.BrandName,
            Color = item.ColorName,
            Size = item.Diameter,
            Quantity = item.RemainingWeightGrams ?? item.TotalSpoolWeightGrams,
            Unit = "g",
            Notes = item.StorageLocation,
            UserId = item.UserId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
