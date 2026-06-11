namespace TankerMade.Contracts.DTOs.ModuleKits;

public class UpdateModuleKitSupplyRequest
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
}
