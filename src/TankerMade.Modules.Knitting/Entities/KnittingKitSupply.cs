namespace TankerMade.Modules.Knitting.Entities;

public class KnittingKitSupply
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingKitSupply() { }

    public KnittingKitSupply(Guid id, Guid kitId, string supplyType, string name, int sortOrder)
    {
        Id = id;
        KitId = kitId;
        SupplyType = supplyType?.Trim() ?? throw new ArgumentNullException(nameof(supplyType));
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(decimal? quantity)
    {
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
