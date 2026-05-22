namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class ReorderCraftingPatternItemsDto
{
    public IReadOnlyList<Guid> OrderedIds { get; set; } = [];
}
