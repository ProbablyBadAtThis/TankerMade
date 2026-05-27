namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class CraftingKitPieceDto
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string PatternName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
