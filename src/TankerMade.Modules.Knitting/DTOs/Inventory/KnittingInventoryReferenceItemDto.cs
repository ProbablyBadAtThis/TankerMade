namespace TankerMade.Modules.Knitting.DTOs.Inventory;

public class KnittingInventoryReferenceItemDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
