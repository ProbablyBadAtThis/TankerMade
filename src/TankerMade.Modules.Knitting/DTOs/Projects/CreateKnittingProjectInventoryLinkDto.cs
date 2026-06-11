namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class CreateKnittingProjectInventoryLinkDto
{
    public Guid SupplyItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
