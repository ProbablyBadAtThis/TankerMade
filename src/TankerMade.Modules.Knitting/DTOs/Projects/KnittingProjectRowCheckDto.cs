namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class KnittingProjectRowCheckDto
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public int RowNumber { get; set; }
}
