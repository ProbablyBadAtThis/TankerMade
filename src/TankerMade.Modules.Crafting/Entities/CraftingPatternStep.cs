namespace TankerMade.Modules.Crafting.Entities;

public class CraftingPatternStep
{
    public Guid Id { get; set; }
    public Guid PatternPieceId { get; set; }
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingPatternStep()
    {
    }

    public CraftingPatternStep(
        Guid id,
        Guid patternPieceId,
        int? rangeStart,
        int? rangeEnd,
        string label,
        string instructions,
        int sortOrder)
    {
        Id = id;
        PatternPieceId = patternPieceId;
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Update(rangeStart, rangeEnd, label, instructions);
    }

    public void Update(int? rangeStart, int? rangeEnd, string label, string instructions)
    {
        if (rangeStart.HasValue && rangeEnd.HasValue && rangeEnd.Value < rangeStart.Value)
        {
            throw new ArgumentException("Range end cannot be before range start.", nameof(rangeEnd));
        }

        RangeStart = rangeStart;
        RangeEnd = rangeEnd;
        Label = label?.Trim() ?? string.Empty;
        Instructions = instructions?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveTo(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
