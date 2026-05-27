namespace TankerMade.Modules.Printing3D.Entities;

public class PrintingInventoryPurchase
{
    public Guid Id { get; set; }
    public Guid MaterialInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    protected PrintingInventoryPurchase()
    {
    }

    public PrintingInventoryPurchase(
        Guid id,
        Guid materialInventoryItemId,
        string sourceName,
        decimal? price,
        bool isSalePrice,
        DateTime? purchasedAt)
    {
        Id = id;
        MaterialInventoryItemId = materialInventoryItemId;
        SourceName = sourceName.Trim();
        Price = price;
        IsSalePrice = isSalePrice;
        PurchasedAt = purchasedAt;
        CreatedAt = DateTime.UtcNow;
    }
}
