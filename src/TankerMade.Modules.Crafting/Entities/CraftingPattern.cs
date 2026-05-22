namespace TankerMade.Modules.Crafting.Entities;

public class CraftingPattern
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public Guid? SourceId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingPattern()
    {
    }

    public CraftingPattern(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Slug = SlugGenerator.Generate(name);
    }

    public void Update(string name, string type, string form, string difficulty, Guid? themeId, Guid? sourceId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? string.Empty;
        Form = form ?? string.Empty;
        Difficulty = difficulty ?? string.Empty;
        ThemeId = themeId;
        SourceId = sourceId;
        Slug = SlugGenerator.Generate(name);
        UpdatedAt = DateTime.UtcNow;
    }
}
