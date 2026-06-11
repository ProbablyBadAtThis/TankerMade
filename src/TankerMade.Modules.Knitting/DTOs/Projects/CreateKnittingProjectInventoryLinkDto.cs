namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class CreateKnittingProjectInventoryLinkDto
{
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
