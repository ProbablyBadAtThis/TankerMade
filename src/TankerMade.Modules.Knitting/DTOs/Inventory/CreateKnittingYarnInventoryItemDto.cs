namespace TankerMade.Modules.Knitting.DTOs.Inventory;

public class CreateKnittingYarnInventoryItemDto
{
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string MainColor { get; set; } = string.Empty;
    public string WeightName { get; set; } = string.Empty;
    public string FiberContent { get; set; } = string.Empty;
    public string FiberTag { get; set; } = string.Empty;
    public decimal Skeins { get; set; }
    public decimal? EstimatedLength { get; set; }
    public string LengthUnit { get; set; } = "yd";
    public decimal? CurrentWeight { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
    public DateTime? PurchasedAt { get; set; }
}
