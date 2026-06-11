namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class CreateModuleProjectInventoryLinkRequest
{
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
