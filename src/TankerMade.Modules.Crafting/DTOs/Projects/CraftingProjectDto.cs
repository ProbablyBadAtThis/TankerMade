namespace TankerMade.Modules.Crafting.DTOs.Projects;

public class CraftingProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string PatternName { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public Guid? KitId { get; set; }
    public string KitName { get; set; } = string.Empty;
    public Guid? KitPieceId { get; set; }
    public string KitPieceName { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int CompletedStepCount { get; set; }
    public int TotalStepCount { get; set; }
    public long TotalTrackedSeconds { get; set; }
    public bool TimerRunning { get; set; }
    public DateTime? TimerStartedAt { get; set; }
    public IReadOnlyList<CraftingProjectStepProgressDto> StepProgress { get; set; } = [];
    public IReadOnlyList<CraftingProjectTimerDto> Timers { get; set; } = [];
    public IReadOnlyList<CraftingProjectInventoryLinkDto> InventoryLinks { get; set; } = [];
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
