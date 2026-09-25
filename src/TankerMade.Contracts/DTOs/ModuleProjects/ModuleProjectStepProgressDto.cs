namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class ModuleProjectStepProgressDto
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
}
