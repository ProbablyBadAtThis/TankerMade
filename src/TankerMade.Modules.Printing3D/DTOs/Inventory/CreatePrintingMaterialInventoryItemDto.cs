namespace TankerMade.Modules.Printing3D.DTOs.Inventory;

public class CreatePrintingMaterialInventoryItemDto
{
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public decimal SpoolWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public string SpoolCode { get; set; } = string.Empty;
    public string PrinterCompatibility { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
}
