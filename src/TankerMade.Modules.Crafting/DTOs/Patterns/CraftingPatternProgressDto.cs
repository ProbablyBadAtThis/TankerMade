namespace TankerMade.Modules.Crafting.DTOs.Patterns;

public class CraftingPatternProgressDto
{
    public int PieceCount { get; set; }
    public int StepCount { get; set; }
    public int EmptyPieceCount { get; set; }
    public int InvalidRangeCount { get; set; }
    public bool HasPieces => PieceCount > 0;
    public bool HasSteps => StepCount > 0;
    public bool IsReadyForProject => HasPieces && HasSteps && EmptyPieceCount == 0 && InvalidRangeCount == 0;
    public IReadOnlyList<string> ValidationMessages { get; set; } = [];
}
