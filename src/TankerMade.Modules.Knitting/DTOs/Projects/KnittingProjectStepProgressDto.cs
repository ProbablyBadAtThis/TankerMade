namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class KnittingProjectStepProgressDto
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
}
