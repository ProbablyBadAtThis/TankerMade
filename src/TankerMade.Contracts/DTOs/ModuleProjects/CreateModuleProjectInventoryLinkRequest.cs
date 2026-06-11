namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class CreateModuleProjectInventoryLinkRequest
{
    public Guid SupplyItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
