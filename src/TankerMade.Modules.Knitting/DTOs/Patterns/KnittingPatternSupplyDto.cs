namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class KnittingPatternSupplyDto
{
    public Guid Id { get; set; }
    public Guid PatternId { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
