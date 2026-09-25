namespace TankerMade.Modules.Knitting.Entities;

public class KnittingProjectStepProgress
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingProjectStepProgress()
    {
    }

    public KnittingProjectStepProgress(Guid id, Guid projectId, Guid patternStepId)
    {
        Id = id;
        ProjectId = projectId;
        PatternStepId = patternStepId;
        CreatedAt = DateTime.UtcNow;
        Complete();
    }

    public void Complete()
    {
        IsComplete = true;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
