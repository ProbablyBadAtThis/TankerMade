namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class KnittingPatternPieceDto
{
    public Guid Id { get; set; }
    public Guid PatternId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public IReadOnlyList<KnittingPatternStepDto> Steps { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
