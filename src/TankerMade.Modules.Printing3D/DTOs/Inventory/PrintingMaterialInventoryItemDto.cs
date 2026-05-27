namespace TankerMade.Modules.Printing3D.DTOs.Inventory;

public class PrintingMaterialInventoryItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public decimal TotalSpoolWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<PrintingSpoolDto> Spools { get; set; } = [];
    public IReadOnlyList<PrintingInventoryPurchaseDto> Purchases { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
