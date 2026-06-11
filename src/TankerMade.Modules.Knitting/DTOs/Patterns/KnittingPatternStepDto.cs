namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class KnittingPatternStepDto
{
    public Guid Id { get; set; }
    public Guid PatternPieceId { get; set; }
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string DisplayRange { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int? StitchCount { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
