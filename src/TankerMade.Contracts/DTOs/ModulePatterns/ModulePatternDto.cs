namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class ModulePatternDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public Guid? SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string SuggestedYarnWeight { get; set; } = string.Empty;
    public string SuggestedNeedleSizes { get; set; } = string.Empty;
    public string RequiredNotions { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public IReadOnlyList<ModulePatternSupplyDto> Supplies { get; set; } = [];
    public IReadOnlyList<ModulePatternPieceDto> Pieces { get; set; } = [];
    public int PieceCount { get; set; }
    public int StepCount { get; set; }
    public ModulePatternProgressDto Progress { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
