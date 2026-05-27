namespace TankerMade.Contracts.DTOs.Modules;

public class ModuleDto
{
    public Guid Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsBundled { get; set; }
    public bool IsActive { get; set; }
    public string NavigationLabel { get; set; } = string.Empty;
    public string NavigationRoute { get; set; } = string.Empty;
    public int NavigationOrder { get; set; }
}
