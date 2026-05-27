namespace TankerMade.Modules.Printing3D.Entities;

public class PrintingMaterialInventoryItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string NormalizedMaterialType { get; set; } = string.Empty;
    public string NormalizedBrandName { get; set; } = string.Empty;
    public string NormalizedColorName { get; set; } = string.Empty;
    public decimal TotalSpoolWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public decimal? RegularPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected PrintingMaterialInventoryItem()
    {
    }

    public PrintingMaterialInventoryItem(
        Guid id,
        Guid userId,
        string materialType,
        string brandName,
        string colorName)
    {
        Id = id;
        UserId = userId;
        MaterialType = Required(materialType, nameof(materialType));
        BrandName = Required(brandName, nameof(brandName));
        ColorName = Required(colorName, nameof(colorName));
        NormalizedMaterialType = Normalize(materialType);
        NormalizedBrandName = Normalize(brandName);
        NormalizedColorName = Normalize(colorName);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MergeDetails(
        decimal spoolWeightGrams,
        decimal? remainingWeightGrams,
        string diameter,
        string storageLocation,
        decimal? price,
        bool isSalePrice)
    {
        TotalSpoolWeightGrams += spoolWeightGrams;
        RemainingWeightGrams = AddNullable(RemainingWeightGrams, remainingWeightGrams);
        Diameter = UseIncoming(diameter, Diameter);
        StorageLocation = UseIncoming(storageLocation, StorageLocation);

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

    private static decimal? AddNullable(decimal? existing, decimal? incoming)
    {
        if (!incoming.HasValue)
        {
            return existing;
        }

        return (existing ?? 0m) + incoming.Value;
    }
}
