namespace TankerMade.Modules.Crafting.DTOs.Projects;

public class CreateCraftingProjectInventoryLinkDto
{
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
