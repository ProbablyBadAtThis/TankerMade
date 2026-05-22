namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class CraftingPatternPieceDto
{
    public Guid Id { get; set; }
    public Guid PatternId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public IReadOnlyList<CraftingPatternStepDto> Steps { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
