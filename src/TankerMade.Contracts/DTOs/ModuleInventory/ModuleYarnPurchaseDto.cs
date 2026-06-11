namespace TankerMade.Contracts.DTOs.ModuleInventory;

public class ModuleYarnPurchaseDto
{
    public Guid Id { get; set; }
    public Guid YarnInventoryItemId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
