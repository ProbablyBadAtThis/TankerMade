namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class CreateModulePatternSupplyRequest
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
}
