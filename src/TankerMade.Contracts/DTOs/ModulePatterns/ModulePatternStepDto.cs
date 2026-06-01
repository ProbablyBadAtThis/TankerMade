namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class ModulePatternStepDto
{
    public Guid Id { get; set; }
    public Guid PieceId { get; set; }
    public int SortOrder { get; set; }
    public string Label { get; set; } = string.Empty;
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }
    public int? StitchCount { get; set; }
    public string Notes { get; set; } = string.Empty;
}
