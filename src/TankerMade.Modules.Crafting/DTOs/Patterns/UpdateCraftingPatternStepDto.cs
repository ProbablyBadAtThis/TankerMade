namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class UpdateCraftingPatternStepDto
{
    public Guid Id { get; set; }
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
}
