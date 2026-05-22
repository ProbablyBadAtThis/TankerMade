namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class CraftingPatternDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public Guid? SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
