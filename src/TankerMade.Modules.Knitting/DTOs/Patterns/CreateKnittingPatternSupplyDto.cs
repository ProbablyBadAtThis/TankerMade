namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class CreateKnittingPatternSupplyDto
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
}
