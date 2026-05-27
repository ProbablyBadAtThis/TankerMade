namespace TankerMade.Modules.Crafting.DTOs.Inventory;

public class CraftingYarnInventoryItemDto
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
    public IReadOnlyList<CraftingYarnLotDto> Lots { get; set; } = [];
    public IReadOnlyList<CraftingInventoryPurchaseDto> Purchases { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
