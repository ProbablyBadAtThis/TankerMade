namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class UpdateModulePatternStepRequest
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }
    public int? StitchCount { get; set; }
    public string? Notes { get; set; }
}
