namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class UpdateCraftingKitPieceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string Notes { get; set; } = string.Empty;
}
