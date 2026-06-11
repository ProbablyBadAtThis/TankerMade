namespace TankerMade.Modules.Knitting.Entities;

public class KnittingInventoryReferenceItem
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    protected KnittingInventoryReferenceItem()
    {
    }

    public KnittingInventoryReferenceItem(Guid id, string category, string name, int sortOrder)
    {
        Id = id;
        Category = category.Trim();
        Name = name.Trim();
        Slug = SlugGenerator.Generate(name);
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
    }
}
