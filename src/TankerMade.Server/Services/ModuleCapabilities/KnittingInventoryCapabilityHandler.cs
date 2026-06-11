using TankerMade.Contracts.DTOs.ModuleInventory;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Inventory;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingInventoryCapabilityHandler : IModuleInventoryCapabilityHandler
{
    private readonly IKnittingInventoryService _service;

    public KnittingInventoryCapabilityHandler(IKnittingInventoryService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleSupplyItemDto>> GetSuppliesAsync(Guid userId, string? search = null, string? category = null)
        => (await _service.GetSuppliesAsync(userId, search, category)).Select(Map).ToList();

    public async Task<ModuleSupplyItemDto> CreateSupplyAsync(CreateModuleSupplyItemRequest request, Guid userId)
        => Map(await _service.CreateSupplyAsync(new CreateKnittingSupplyItemDto
        {
            Category = request.Category,
            Name = request.Name,
            Brand = request.Brand,
            Color = request.Color,
            Size = request.Size,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes
        }, userId));

    public async Task<ModuleSupplyItemDto?> UpdateSupplyAsync(Guid supplyId, UpdateModuleSupplyItemRequest request, Guid userId)
    {
        var supply = await _service.UpdateSupplyAsync(supplyId, new CreateKnittingSupplyItemDto
        {
            Category = request.Category,
            Name = request.Name,
            Brand = request.Brand,
            Color = request.Color,
            Size = request.Size,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes
        }, userId);

        return supply == null ? null : Map(supply);
    }

    public Task<bool> DeleteSupplyAsync(Guid supplyId, Guid userId)
        => _service.DeleteSupplyAsync(supplyId, userId);

    private static ModuleSupplyItemDto Map(KnittingSupplyItemDto source)
    {
        return new ModuleSupplyItemDto
        {
            Id = source.Id,
            Category = source.Category,
            Name = source.Name,
            Brand = source.Brand,
            Color = source.Color,
            Size = source.Size,
            Quantity = source.Quantity,
            Unit = source.Unit,
            Notes = source.Notes,
            UserId = source.UserId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
