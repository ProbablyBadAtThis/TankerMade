namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class ModuleProjectInventoryLinkDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SupplyItemId { get; set; }
    public string SupplyItemName { get; set; } = string.Empty;
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
