namespace TankerMade.Modules.Crafting.Entities;

public class CraftingProject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public int Difficulty { get; set; }
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingProject()
    {
    }

    public CraftingProject(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Progress = 0;
        Slug = SlugGenerator.Generate(name);
    }

    public void Update(string name, string description, Guid? patternId, Guid? themeId, int difficulty, int progress)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        PatternId = patternId;
        ThemeId = themeId;
        Difficulty = difficulty;
        SetProgress(progress);
        Slug = SlugGenerator.Generate(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetProgress(int progress)
    {
        if (progress < 0 || progress > 100)
        {
            throw new ArgumentException("Progress must be between 0 and 100.", nameof(progress));
        }

        Progress = progress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive(DateTime archivedAt)
    {
        IsArchived = true;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;
    }

    public void Reopen(DateTime reopenedAt)
    {
        IsArchived = false;
        ArchivedAt = null;
        UpdatedAt = reopenedAt;
    }
}
