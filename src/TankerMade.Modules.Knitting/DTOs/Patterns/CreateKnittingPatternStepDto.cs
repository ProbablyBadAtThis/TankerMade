namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class CreateKnittingPatternStepDto
{
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string Label { get; set; } = string.Empty;
    public int? StitchCount { get; set; }
    public string Instructions { get; set; } = string.Empty;
}
