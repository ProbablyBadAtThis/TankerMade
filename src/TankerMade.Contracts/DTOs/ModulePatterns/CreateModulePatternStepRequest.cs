namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class CreateModulePatternStepRequest
{
    public string Label { get; set; } = string.Empty;
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }
    public int? StitchCount { get; set; }
    public string Notes { get; set; } = string.Empty;
}
