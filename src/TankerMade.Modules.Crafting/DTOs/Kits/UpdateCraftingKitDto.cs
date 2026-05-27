namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class UpdateCraftingKitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public int? Difficulty { get; set; }
    public int? Progress { get; set; }
}
