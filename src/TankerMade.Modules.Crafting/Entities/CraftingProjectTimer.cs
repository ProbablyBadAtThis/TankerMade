namespace TankerMade.Modules.Crafting.Entities;

public class CraftingProjectTimer
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public long ElapsedSeconds { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingProjectTimer()
    {
    }

    public CraftingProjectTimer(Guid id, Guid projectId, Guid patternStepId)
    {
        Id = id;
        ProjectId = projectId;
        PatternStepId = patternStepId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start(DateTime startedAt)
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        StartedAt = startedAt;
        UpdatedAt = startedAt;
    }

    public void Pause(DateTime pausedAt)
    {
        if (!IsRunning || StartedAt == null)
        {
            IsRunning = false;
            StartedAt = null;
            UpdatedAt = pausedAt;
            return;
        }

        ElapsedSeconds += Math.Max(0, (long)(pausedAt - StartedAt.Value).TotalSeconds);
        IsRunning = false;
        StartedAt = null;
        UpdatedAt = pausedAt;
    }

    public void SetElapsedSeconds(long elapsedSeconds, DateTime updatedAt)
    {
        ElapsedSeconds = Math.Max(0, elapsedSeconds);
        if (IsRunning)
        {
            StartedAt = updatedAt;
        }

        UpdatedAt = updatedAt;
    }

    public void Reset(DateTime updatedAt)
    {
        SetElapsedSeconds(0, updatedAt);
    }

    public long GetElapsedSeconds(DateTime asOf)
    {
        if (!IsRunning || StartedAt == null)
        {
            return ElapsedSeconds;
        }

        return ElapsedSeconds + Math.Max(0, (long)(asOf - StartedAt.Value).TotalSeconds);
    }
}
