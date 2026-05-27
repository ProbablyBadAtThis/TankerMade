namespace TankerMade.Modules.Crafting.DTOs.Kits;

public class CraftingKitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public IReadOnlyList<CraftingKitPieceDto> Pieces { get; set; } = [];
    public IReadOnlyList<CraftingKitSupplyDto> Supplies { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
