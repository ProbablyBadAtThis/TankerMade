namespace TankerMade.Modules.Crafting.Entities;

public class CraftingYarnInventoryItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string NormalizedBrandName { get; set; } = string.Empty;
    public string NormalizedColorName { get; set; } = string.Empty;
    public string MainColor { get; set; } = string.Empty;
    public string WeightName { get; set; } = string.Empty;
    public string FiberContent { get; set; } = string.Empty;
    public string FiberTag { get; set; } = string.Empty;
    public decimal TotalSkeins { get; set; }
    public decimal? EstimatedRemainingLength { get; set; }
    public string LengthUnit { get; set; } = "yd";
    public decimal? CurrentWeight { get; set; }
    public decimal? RegularPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingYarnInventoryItem()
    {
    }

    public CraftingYarnInventoryItem(
        Guid id,
        Guid userId,
        string brandName,
        string colorName)
    {
        Id = id;
        UserId = userId;
        BrandName = Required(brandName, nameof(brandName));
        ColorName = Required(colorName, nameof(colorName));
        NormalizedBrandName = Normalize(brandName);
        NormalizedColorName = Normalize(colorName);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MergeDetails(
        string mainColor,
        string weightName,
        string fiberContent,
        string fiberTag,
        decimal skeins,
        decimal? estimatedRemainingLength,
        string lengthUnit,
        decimal? currentWeight,
        decimal? price,
        bool isSalePrice)
    {
        MainColor = UseIncoming(mainColor, MainColor);
        WeightName = UseIncoming(weightName, WeightName);
        FiberContent = UseIncoming(fiberContent, FiberContent);
        FiberTag = UseIncoming(fiberTag, FiberTag);
        LengthUnit = UseIncoming(lengthUnit, LengthUnit);
        TotalSkeins += skeins;
        EstimatedRemainingLength = AddNullable(EstimatedRemainingLength, estimatedRemainingLength);
        CurrentWeight = currentWeight ?? CurrentWeight;

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
