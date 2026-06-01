namespace TankerMade.Modules.Knitting.DTOs.Patterns;

public class ReorderKnittingPatternItemsDto
{
    public IReadOnlyList<Guid> OrderedIds { get; set; } = [];
}
