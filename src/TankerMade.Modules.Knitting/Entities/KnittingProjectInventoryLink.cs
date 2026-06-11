namespace TankerMade.Modules.Knitting.Entities;

public class KnittingProjectInventoryLink
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SupplyItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingProjectInventoryLink()
    {
    }

    public KnittingProjectInventoryLink(
        Guid id,
        Guid projectId,
        Guid supplyItemId,
        decimal? quantityPlanned,
        string notes)
    {
        Id = id;
        ProjectId = projectId;
        SupplyItemId = supplyItemId;
        QuantityPlanned = quantityPlanned;
        Notes = notes?.Trim() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(decimal? quantityPlanned, string notes)
    {
        QuantityPlanned = quantityPlanned;
        Notes = notes?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}
