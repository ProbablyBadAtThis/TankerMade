namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class UpdateCraftingKitSupplyDto
{
    public Guid Id { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}
