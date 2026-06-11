namespace TankerMade.Modules.Knitting.Entities;

public class KnittingNotionInventoryItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string NormalizedBrandName { get; set; } = string.Empty;
    public string NormalizedTypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? RegularPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingNotionInventoryItem()
    {
    }

    public KnittingNotionInventoryItem(Guid id, Guid userId, string brandName, string typeName)
    {
        Id = id;
        UserId = userId;
        BrandName = Required(brandName, nameof(brandName));
        TypeName = Required(typeName, nameof(typeName));
        NormalizedBrandName = Normalize(brandName);
        NormalizedTypeName = Normalize(typeName);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MergeDetails(
        string size,
        string colorName,
        string description,
        int quantity,
        decimal? price,
        bool isSalePrice)
    {
        Size = UseIncoming(size, Size);
        ColorName = UseIncoming(colorName, ColorName);
        Description = UseIncoming(description, Description);
        Quantity += quantity;

        if (price.HasValue && !isSalePrice)
        {
            RegularPrice = price;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static string UseIncoming(string incoming, string existing)
    {
        return string.IsNullOrWhiteSpace(incoming) ? existing : incoming.Trim();
    }
}
