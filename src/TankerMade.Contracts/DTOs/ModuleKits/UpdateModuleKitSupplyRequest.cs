namespace TankerMade.Contracts.DTOs.ModuleKits;

public class UpdateModuleKitSupplyRequest
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
    public decimal? Quantity { get; set; }
}
