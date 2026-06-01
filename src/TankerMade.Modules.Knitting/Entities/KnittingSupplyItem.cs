namespace TankerMade.Modules.Knitting.Entities;

public class KnittingSupplyItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingSupplyItem()
    {
    }

    public KnittingSupplyItem(Guid id, Guid userId, string category, string name)
    {
        Id = id;
        UserId = userId;
        Category = Normalize(category);
        Name = Normalize(name);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string brand, string color, string size, decimal quantity, string unit, string notes)
    {
        Brand = brand?.Trim() ?? string.Empty;
        Color = color?.Trim() ?? string.Empty;
        Size = size?.Trim() ?? string.Empty;
        Quantity = quantity < 0 ? 0 : quantity;
        Unit = unit?.Trim() ?? string.Empty;
        Notes = notes?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        return value.Trim();
    }
}
