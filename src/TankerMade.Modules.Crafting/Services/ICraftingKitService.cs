using TankerMade.Modules.Crafting.DTOs.Kits;
using TankerMade.Modules.Crafting.DTOs.Projects;

namespace TankerMade.Modules.Crafting.Services;

public interface ICraftingKitService
{
    Task<CraftingKitDto> CreateAsync(CreateCraftingKitDto createDto, Guid userId);
    Task<CraftingKitDto?> GetByIdAsync(Guid id, Guid userId);
    Task<IReadOnlyList<CraftingKitDto>> GetAllAsync(Guid userId, bool includeArchived = false);
    Task<CraftingKitDto?> UpdateAsync(UpdateCraftingKitDto updateDto, Guid userId);
    Task<CraftingKitDto?> ArchiveAsync(Guid id, Guid userId);
    Task<CraftingKitDto?> ReopenAsync(Guid id, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<CraftingKitPieceDto?> AddPieceAsync(Guid kitId, CreateCraftingKitPieceDto createDto, Guid userId);
    Task<CraftingKitPieceDto?> UpdatePieceAsync(Guid kitId, UpdateCraftingKitPieceDto updateDto, Guid userId);
    Task<bool> DeletePieceAsync(Guid kitId, Guid pieceId, Guid userId);
    Task<bool> ReorderPiecesAsync(Guid kitId, ReorderCraftingKitItemsDto reorderDto, Guid userId);
    Task<CraftingProjectDto?> CreateProjectForPieceAsync(Guid kitId, Guid pieceId, CreateCraftingKitProjectDto createDto, Guid userId);
    Task<CraftingKitSupplyDto?> AddSupplyAsync(Guid kitId, CreateCraftingKitSupplyDto createDto, Guid userId);
    Task<CraftingKitSupplyDto?> UpdateSupplyAsync(Guid kitId, UpdateCraftingKitSupplyDto updateDto, Guid userId);
    Task<bool> DeleteSupplyAsync(Guid kitId, Guid supplyId, Guid userId);
    Task<bool> ReorderSuppliesAsync(Guid kitId, ReorderCraftingKitItemsDto reorderDto, Guid userId);
}
