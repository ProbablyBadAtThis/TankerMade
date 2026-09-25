namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class ModuleProjectRowCheckDto
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public int RowNumber { get; set; }
}
