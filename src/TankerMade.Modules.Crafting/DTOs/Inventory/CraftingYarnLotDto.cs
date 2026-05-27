namespace TankerMade.Modules.Crafting.DTOs.Inventory;

public class CraftingYarnLotDto
{
    public Guid Id { get; set; }
    public Guid YarnInventoryItemId { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public decimal Skeins { get; set; }
    public decimal? RemainingLength { get; set; }
    public decimal? CurrentWeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
