namespace TankerMade.Modules.Knitting.Entities;

public class KnittingKitPiece
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected KnittingKitPiece() { }

    public KnittingKitPiece(Guid id, Guid kitId, string name, int sortOrder)
    {
        Id = id;
        KitId = kitId;
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string notes)
    {
        Notes = notes?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkProject(Guid projectId)
    {
        ProjectId = projectId;
        UpdatedAt = DateTime.UtcNow;
    }
}
