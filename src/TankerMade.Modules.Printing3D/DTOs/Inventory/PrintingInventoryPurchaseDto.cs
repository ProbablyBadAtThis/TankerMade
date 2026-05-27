namespace TankerMade.Modules.Printing3D.DTOs.Inventory;

public class PrintingInventoryPurchaseDto
{
    public Guid Id { get; set; }
    public Guid MaterialInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
