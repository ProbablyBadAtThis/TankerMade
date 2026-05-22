namespace TankerMade.Modules.Crafting.DTOs.Projects;

public class CraftingProjectStepProgressDto
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
}
