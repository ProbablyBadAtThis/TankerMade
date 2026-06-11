namespace TankerMade.Modules.Knitting.Entities;

public class KnittingPatternSupply
{
    public Guid Id { get; set; }
    public Guid PatternId { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InventoryItemId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingPatternSupply()
    {
    }

    public KnittingPatternSupply(Guid id, Guid patternId, string supplyType, string name, int sortOrder)
    {
        Id = id;
        PatternId = patternId;
        SupplyType = NormalizeSupplyType(supplyType);
        Name = NormalizeName(name);
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string supplyType, string name, Guid? inventoryItemId)
    {
        SupplyType = NormalizeSupplyType(supplyType);
        Name = NormalizeName(name);
        InventoryItemId = inventoryItemId;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeSupplyType(string supplyType)
    {
        return supplyType?.Trim().ToLowerInvariant() switch
        {
            "tool" => "tool",
            "notion" => "notion",
            _ => "yarn"
        };
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Supply name is required.", nameof(name));
        }

        return name.Trim();
    }
}
