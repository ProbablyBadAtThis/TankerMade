namespace TankerMade.Modules.Knitting.DTOs.Projects;

public class CreateKnittingProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public Guid? ColorId { get; set; }
    public int Difficulty { get; set; }
    public int? Progress { get; set; }
    public DateTime? StartedAt { get; set; }
}
