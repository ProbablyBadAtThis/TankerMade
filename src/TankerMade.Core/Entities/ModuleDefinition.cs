namespace TankerMade.Core.Entities;

public class ModuleDefinition
{
    public Guid Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsBundled { get; set; }
    public DateTime CreatedAt { get; set; }

    protected ModuleDefinition()
    {
    }

    public ModuleDefinition(Guid id, string moduleKey, string name, string description, string version, bool isBundled)
    {
        Id = id;
        ModuleKey = moduleKey ?? throw new ArgumentNullException(nameof(moduleKey));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        IsBundled = isBundled;
        CreatedAt = DateTime.UtcNow;
    }
}
