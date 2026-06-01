namespace TankerMade.Modules.Knitting.DTOs.Kits;

public class CreateKnittingKitSupplyDto
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
}
