namespace TankerMade.Modules.Crafting.DTOs.Inventory;

public class CraftingToolInventoryItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<CraftingToolPurchaseDto> Purchases { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
