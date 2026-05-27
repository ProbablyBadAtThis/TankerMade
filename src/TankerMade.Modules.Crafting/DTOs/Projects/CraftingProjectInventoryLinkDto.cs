namespace TankerMade.Modules.Crafting.DTOs.Projects;

public class CraftingProjectInventoryLinkDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
