namespace TankerMade.Contracts.DTOs.Projects;

public class ProjectResponse
{
    // Core project data
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Pattern relationship (nullable since it's optional)
    public Guid? PatternId { get; set; }
    public string? PatternName { get; set; }  // Flattened - nullable because PatternId is optional

    // Theme relationship (nullable since it's optional)
    public Guid? ThemeId { get; set; }
    public string? ThemeName { get; set; }    // Flattened - nullable because ThemeId is optional

    // Project details
    public int Difficulty { get; set; }
    public string DifficultyName { get; set; } = string.Empty;  // Enum display value
    public decimal Progress { get; set; }

    // Ownership
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;  // Flattened from User

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
