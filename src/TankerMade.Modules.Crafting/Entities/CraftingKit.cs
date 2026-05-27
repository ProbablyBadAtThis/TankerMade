namespace TankerMade.Modules.Crafting.Entities;

public class CraftingKit
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public int Difficulty { get; set; }
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingKit()
    {
    }

    public CraftingKit(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = NormalizeName(name);
        Slug = SlugGenerator.Generate(Name);
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, string type, Guid? themeId, int difficulty, int progress)
    {
        Name = NormalizeName(name);
        Description = description?.Trim() ?? string.Empty;
        Type = type?.Trim() ?? string.Empty;
        ThemeId = themeId;
        Difficulty = difficulty;
        SetProgress(progress);
        Slug = SlugGenerator.Generate(Name);
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

    private void SetProgress(int progress)
    {
        if (progress < 0 || progress > 100)
        {
            throw new ArgumentException("Progress must be between 0 and 100.", nameof(progress));
        }

        Progress = progress;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kit name is required.", nameof(name));
        }

        return name.Trim();
    }
}
