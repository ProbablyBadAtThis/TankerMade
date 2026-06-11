namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class UpdateModulePatternRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Form { get; set; }
    public string? Difficulty { get; set; }
    public Guid? ThemeId { get; set; }
    public Guid? SourceId { get; set; }
    public string? SuggestedYarnWeight { get; set; }
    public string? SuggestedNeedleSizes { get; set; }
    public string? RequiredNotions { get; set; }
}
