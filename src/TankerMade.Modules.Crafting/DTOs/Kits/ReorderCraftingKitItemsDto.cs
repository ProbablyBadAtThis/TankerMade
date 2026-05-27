namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class ReorderCraftingKitItemsDto
{
    public IReadOnlyList<Guid> OrderedIds { get; set; } = [];
}
