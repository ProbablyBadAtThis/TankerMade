namespace TankerMade.Modules.Printing3D.Entities;

public class PrintingInventoryReferenceItem
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    protected PrintingInventoryReferenceItem()
    {
    }

    public PrintingInventoryReferenceItem(Guid id, string category, string name, int sortOrder)
    {
        Id = id;
        Category = category.Trim();
        Name = name.Trim();
        Slug = GenerateSlug(name);
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
    }

    private static string GenerateSlug(string value)
    {
        return value.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "-")
            .Replace("/", "-")
            .Replace("'", string.Empty);
    }
}
