namespace TankerMade.Modules.Knitting.Entities;

public class KnittingProjectInventoryLink
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
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
        string inventoryItemType,
        Guid inventoryItemId,
        decimal? quantityPlanned,
        string notes)
    {
        Id = id;
        ProjectId = projectId;
        InventoryItemType = inventoryItemType.Trim();
        InventoryItemId = inventoryItemId;
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
