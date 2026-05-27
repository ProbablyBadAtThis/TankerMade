namespace TankerMade.Modules.Crafting.Entities;

public class CraftingKitSupply
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingKitSupply()
    {
    }

    public CraftingKitSupply(Guid id, Guid kitId, string supplyType, string name, decimal? quantity, string notes, int sortOrder)
    {
        Id = id;
        KitId = kitId;
        SupplyType = NormalizeSupplyType(supplyType);
        Name = NormalizeName(name);
        Quantity = NormalizeQuantity(quantity);
        Notes = notes?.Trim() ?? string.Empty;
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string supplyType, string name, decimal? quantity, string notes)
    {
        SupplyType = NormalizeSupplyType(supplyType);
        Name = NormalizeName(name);
        Quantity = NormalizeQuantity(quantity);
        Notes = notes?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveTo(int sortOrder)
    {
        SortOrder = sortOrder;
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

    private static decimal? NormalizeQuantity(decimal? quantity)
    {
        if (quantity is <= 0)
        {
            throw new ArgumentException("Supply quantity must be greater than 0 when provided.", nameof(quantity));
        }

        return quantity;
    }
}
