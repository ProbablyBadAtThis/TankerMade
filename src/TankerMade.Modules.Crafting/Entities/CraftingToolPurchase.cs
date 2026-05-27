namespace TankerMade.Modules.Crafting.Entities;

public class CraftingToolPurchase
{
    public Guid Id { get; set; }
    public Guid ToolInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    protected CraftingToolPurchase()
    {
    }

    public CraftingToolPurchase(
        Guid id,
        Guid toolInventoryItemId,
        string sourceName,
        decimal? price,
        bool isSalePrice,
        DateTime? purchasedAt)
    {
        Id = id;
        ToolInventoryItemId = toolInventoryItemId;
        SourceName = sourceName.Trim();
        Price = price;
        IsSalePrice = isSalePrice;
        PurchasedAt = purchasedAt;
        CreatedAt = DateTime.UtcNow;
    }
}
