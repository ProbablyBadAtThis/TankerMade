namespace TankerMade.Modules.Crafting.DTOs.Inventory;

public class CraftingToolPurchaseDto
{
    public Guid Id { get; set; }
    public Guid ToolInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
