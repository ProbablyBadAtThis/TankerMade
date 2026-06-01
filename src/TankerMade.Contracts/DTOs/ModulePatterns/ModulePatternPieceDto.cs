namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class ModulePatternPieceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int StepCount { get; set; }
    public int CompletedStepCount { get; set; }
    public bool IsComplete { get; set; }
    public IReadOnlyList<ModulePatternStepDto> Steps { get; set; } = [];
}
