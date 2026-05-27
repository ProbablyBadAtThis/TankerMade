namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class CreateCraftingKitProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public int? Difficulty { get; set; }
}
