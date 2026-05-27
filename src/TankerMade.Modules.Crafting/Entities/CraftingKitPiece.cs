namespace TankerMade.Modules.Crafting.Entities;

public class CraftingKitPiece
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingKitPiece()
    {
    }

    public CraftingKitPiece(Guid id, Guid kitId, string name, Guid? patternId, string notes, int sortOrder)
    {
        Id = id;
        KitId = kitId;
        Name = NormalizeName(name);
        PatternId = patternId;
        Notes = notes?.Trim() ?? string.Empty;
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, Guid? patternId, string notes)
    {
        Name = NormalizeName(name);
        PatternId = patternId;
        Notes = notes?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveTo(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kit piece name is required.", nameof(name));
        }

        return name.Trim();
    }
}
