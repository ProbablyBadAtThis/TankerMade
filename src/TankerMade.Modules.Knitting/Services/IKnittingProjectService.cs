using TankerMade.Modules.Knitting.DTOs.Projects;

namespace TankerMade.Modules.Knitting.Services;

public interface IKnittingProjectService
{
    Task<KnittingProjectDto> CreateAsync(CreateKnittingProjectDto createDto, Guid userId);
    Task<IReadOnlyList<KnittingProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false, int page = 1, int pageSize = 50);
    Task<IReadOnlyList<KnittingProjectDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = 50);
    Task<KnittingProjectDto?> GetByIdAsync(Guid id, Guid userId);
    Task<KnittingProjectDto?> UpdateAsync(UpdateKnittingProjectDto updateDto, Guid userId);
    Task<KnittingProjectDto?> ArchiveAsync(Guid id, Guid userId);
    Task<KnittingProjectDto?> ReopenAsync(Guid id, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<KnittingProjectDto?> SetStepProgressAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectStepProgressDto updateDto, Guid userId);
    Task<KnittingProjectDto?> StartTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId);
    Task<KnittingProjectDto?> PauseTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId);
    Task<KnittingProjectDto?> SetTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId);
    Task<KnittingProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId);
    Task<KnittingProjectDto?> AddInventoryLinkAsync(Guid projectId, CreateKnittingProjectInventoryLinkDto createDto, Guid userId);
    Task<bool> RemoveInventoryLinkAsync(Guid projectId, Guid linkId, Guid userId);
}
