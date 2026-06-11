namespace TankerMade.Modules.Knitting.DTOs.Inventory;

public class KnittingYarnInventoryItemDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string MainColor { get; set; } = string.Empty;
    public string WeightName { get; set; } = string.Empty;
    public string FiberContent { get; set; } = string.Empty;
    public string FiberTag { get; set; } = string.Empty;
    public decimal TotalSkeins { get; set; }
    public decimal? EstimatedRemainingLength { get; set; }
    public string LengthUnit { get; set; } = string.Empty;
    public decimal? CurrentWeight { get; set; }
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<KnittingYarnLotDto> Lots { get; set; } = [];
    public IReadOnlyList<KnittingYarnPurchaseDto> Purchases { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
