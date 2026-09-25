namespace TankerMade.Modules.Knitting.Entities;

public class KnittingNotionPurchase
{
    public Guid Id { get; set; }
    public Guid NotionInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    protected KnittingNotionPurchase()
    {
    }

    public KnittingNotionPurchase(
        Guid id,
        Guid notionInventoryItemId,
        string sourceName,
        decimal? price,
        bool isSalePrice,
        DateTime? purchasedAt)
    {
        Id = id;
        NotionInventoryItemId = notionInventoryItemId;
        SourceName = sourceName.Trim();
        Price = price;
        IsSalePrice = isSalePrice;
        PurchasedAt = purchasedAt;
        CreatedAt = DateTime.UtcNow;
    }
}
