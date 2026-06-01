namespace TankerMade.Contracts.DTOs.ModulePatterns;

public class ReorderModulePatternItemsRequest
{
    public IReadOnlyList<Guid> OrderedIds { get; set; } = [];
}
