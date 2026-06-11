namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class KnittingProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string PatternName { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int CompletedStepCount { get; set; }
    public int TotalStepCount { get; set; }
    public long TotalTrackedSeconds { get; set; }
    public bool TimerRunning { get; set; }
    public DateTime? TimerStartedAt { get; set; }
    public IReadOnlyList<KnittingProjectStepProgressDto> StepProgress { get; set; } = [];
    public IReadOnlyList<KnittingProjectTimerDto> Timers { get; set; } = [];
    public IReadOnlyList<KnittingProjectInventoryLinkDto> InventoryLinks { get; set; } = [];
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
