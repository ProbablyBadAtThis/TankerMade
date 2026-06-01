namespace TankerMade.Contracts.DTOs.ModuleProjects;

public class UpdateModuleProjectRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? PatternId { get; set; }
    public bool ClearPatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public int? Difficulty { get; set; }
    public int? Progress { get; set; }
}
