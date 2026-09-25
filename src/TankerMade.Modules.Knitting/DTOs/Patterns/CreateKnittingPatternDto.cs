namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class CreateKnittingPatternDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public Guid? ColorId { get; set; }
    public Guid? SourceId { get; set; }
    public string SuggestedYarnWeight { get; set; } = string.Empty;
    public string SuggestedNeedleSizes { get; set; } = string.Empty;
    public string RequiredNotions { get; set; } = string.Empty;
}
