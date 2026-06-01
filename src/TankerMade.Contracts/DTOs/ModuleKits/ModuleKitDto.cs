namespace TankerMade.Contracts.DTOs.ModuleKits;

public class ModuleKitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public IReadOnlyList<ModuleKitPieceDto> Pieces { get; set; } = [];
    public IReadOnlyList<ModuleKitSupplyDto> Supplies { get; set; } = [];
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
