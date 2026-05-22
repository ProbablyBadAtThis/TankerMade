namespace TankerMade.Modules.Crafting.Entities;

public class CraftingPatternPiece
{
    public Guid Id { get; set; }
    public Guid PatternId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CraftingPatternPiece()
    {
    }

    public CraftingPatternPiece(Guid id, Guid patternId, string name, int sortOrder)
    {
        Id = id;
        PatternId = patternId;
        Name = NormalizeName(name);
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name)
    {
        Name = NormalizeName(name);
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
            throw new ArgumentException("Piece name is required.", nameof(name));
        }

        return name.Trim();
    }
}
