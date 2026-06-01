namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class ModulePatternProgressDto
{
    public int TotalPieces { get; set; }
    public int CompletedPieces { get; set; }
    public int TotalSteps { get; set; }
    public int CompletedSteps { get; set; }
    public int CompletionPercent { get; set; }
}
