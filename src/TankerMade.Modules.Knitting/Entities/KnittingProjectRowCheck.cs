namespace TankerMade.Modules.Knitting.Entities;

public class KnittingProjectRowCheck
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public int RowNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    protected KnittingProjectRowCheck()
    {
    }

    public KnittingProjectRowCheck(Guid id, Guid projectId, Guid patternStepId, int rowNumber)
    {
        Id = id;
        ProjectId = projectId;
        PatternStepId = patternStepId;
        RowNumber = rowNumber;
        CreatedAt = DateTime.UtcNow;
    }
}
