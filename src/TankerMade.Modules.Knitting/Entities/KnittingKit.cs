namespace TankerMade.Modules.Knitting.Entities;

public class KnittingKit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingKit() { }

    public KnittingKit(Guid id, Guid userId, string name)
    {
        Id = id;
        UserId = userId;
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string description, string type)
    {
        Description = description?.Trim() ?? string.Empty;
        Type = type?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        ArchivedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        IsArchived = false;
        ArchivedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
