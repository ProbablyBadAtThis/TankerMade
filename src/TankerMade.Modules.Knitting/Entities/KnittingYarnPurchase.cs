namespace TankerMade.Modules.Knitting.Entities;

public class KnittingYarnPurchase
{
    public Guid Id { get; set; }
    public Guid YarnInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    protected KnittingYarnPurchase()
    {
    }

    public KnittingYarnPurchase(
        Guid id,
        Guid yarnInventoryItemId,
        string sourceName,
        decimal? price,
        bool isSalePrice,
        DateTime? purchasedAt)
    {
        Id = id;
        YarnInventoryItemId = yarnInventoryItemId;
        SourceName = sourceName.Trim();
        Price = price;
        IsSalePrice = isSalePrice;
        PurchasedAt = purchasedAt;
        CreatedAt = DateTime.UtcNow;
    }
}
