namespace TankerMade.Modules.Knitting.Entities;

public class KnittingYarnLot
{
    public Guid Id { get; set; }
    public Guid YarnInventoryItemId { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public decimal Skeins { get; set; }
    public decimal? RemainingLength { get; set; }
    public decimal? CurrentWeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingYarnLot()
    {
    }

    public KnittingYarnLot(
        Guid id,
        Guid yarnInventoryItemId,
        string lotNumber,
        decimal skeins,
        decimal? remainingLength,
        decimal? currentWeight)
    {
        Id = id;
        YarnInventoryItemId = yarnInventoryItemId;
        LotNumber = lotNumber.Trim();
        Skeins = skeins;
        RemainingLength = remainingLength;
        CurrentWeight = currentWeight;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRemaining(decimal? remainingLength, decimal? currentWeight)
    {
        RemainingLength = remainingLength;
        CurrentWeight = currentWeight;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Merge(decimal skeins, decimal? remainingLength, decimal? currentWeight)
    {
        Skeins += skeins;
        RemainingLength = AddNullable(RemainingLength, remainingLength);
        CurrentWeight = currentWeight ?? CurrentWeight;
        UpdatedAt = DateTime.UtcNow;
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
