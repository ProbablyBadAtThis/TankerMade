namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class KnittingProjectTimerDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public long ElapsedSeconds { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
