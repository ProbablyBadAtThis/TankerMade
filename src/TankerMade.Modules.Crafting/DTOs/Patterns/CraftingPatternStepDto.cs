namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class CraftingPatternStepDto
{
    public Guid Id { get; set; }
    public Guid PatternPieceId { get; set; }
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string DisplayRange { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
