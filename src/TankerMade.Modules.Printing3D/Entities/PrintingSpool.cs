namespace TankerMade.Modules.Printing3D.Entities;

public class PrintingSpool
{
    public Guid Id { get; set; }
    public Guid MaterialInventoryItemId { get; set; }
    public string SpoolCode { get; set; } = string.Empty;
    public decimal StartingWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string PrinterCompatibility { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected PrintingSpool()
    {
    }

    public PrintingSpool(
        Guid id,
        Guid materialInventoryItemId,
        string spoolCode,
        decimal startingWeightGrams,
        decimal? remainingWeightGrams,
        string printerCompatibility)
    {
        Id = id;
        MaterialInventoryItemId = materialInventoryItemId;
        SpoolCode = spoolCode.Trim();
        StartingWeightGrams = startingWeightGrams;
        RemainingWeightGrams = remainingWeightGrams;
        PrinterCompatibility = printerCompatibility.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Merge(decimal startingWeightGrams, decimal? remainingWeightGrams, string printerCompatibility)
    {
        StartingWeightGrams += startingWeightGrams;
        RemainingWeightGrams = AddNullable(RemainingWeightGrams, remainingWeightGrams);
        PrinterCompatibility = string.IsNullOrWhiteSpace(printerCompatibility)
            ? PrinterCompatibility
            : printerCompatibility.Trim();
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
