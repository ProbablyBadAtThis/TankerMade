using TankerMade.Contracts.DTOs.ModuleProjects;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Modules.Knitting.Services;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingProjectCapabilityHandler : IModuleProjectCapabilityHandler
{
    private readonly IKnittingProjectService _service;

    public KnittingProjectCapabilityHandler(IKnittingProjectService service)
    {
        _service = service;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<ModuleProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false, int page = 1, int pageSize = 50)
        => (await _service.GetAllAsync(userId, includeArchived, page, pageSize)).Select(Map).ToList();

    public async Task<ModuleProjectDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var result = await _service.GetByIdAsync(id, userId);
        return result == null ? null : Map(result);
    }

    public async Task<IReadOnlyList<ModuleProjectDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = 50)
        => (await _service.SearchAsync(query, userId, page, pageSize)).Select(Map).ToList();

    public async Task<ModuleProjectDto> CreateAsync(CreateModuleProjectRequest request, Guid userId)
        => Map(await _service.CreateAsync(new CreateKnittingProjectDto
        {
            Name = request.Name,
            Description = request.Description,
            PatternId = request.PatternId,
            ThemeId = request.ThemeId,
            Difficulty = request.Difficulty,
            Progress = request.Progress
        }, userId));

    public async Task<ModuleProjectDto?> UpdateAsync(UpdateModuleProjectRequest request, Guid userId)
    {
        var updated = await _service.UpdateAsync(new UpdateKnittingProjectDto
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            PatternId = request.PatternId,
            ClearPatternId = request.ClearPatternId,
            ThemeId = request.ThemeId,
            Difficulty = request.Difficulty,
            Progress = request.Progress
        }, userId);

        return updated == null ? null : Map(updated);
    }

    public async Task<ModuleProjectDto?> ArchiveAsync(Guid id, Guid userId)
    {
        var archived = await _service.ArchiveAsync(id, userId);
        return archived == null ? null : Map(archived);
    }

    public async Task<ModuleProjectDto?> ReopenAsync(Guid id, Guid userId)
    {
        var reopened = await _service.ReopenAsync(id, userId);
        return reopened == null ? null : Map(reopened);
    }

    public Task<bool> DeleteAsync(Guid id, Guid userId) => _service.DeleteAsync(id, userId);

    public async Task<ModuleProjectDto?> SetStepProgressAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectStepProgressRequest request, Guid userId)
    {
        var project = await _service.SetStepProgressAsync(projectId, patternStepId, new UpdateKnittingProjectStepProgressDto
        {
            IsComplete = request.IsComplete
        }, userId);

        return project == null ? null : Map(project);
    }

    public async Task<ModuleProjectDto?> StartTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId)
    {
        var project = await _service.StartTimerAsync(projectId, patternStepId, new UpdateKnittingProjectTimerDto
        {
            ElapsedSeconds = request.ElapsedSeconds
        }, userId);

        return project == null ? null : Map(project);
    }

    public async Task<ModuleProjectDto?> PauseTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId)
    {
        var project = await _service.PauseTimerAsync(projectId, patternStepId, new UpdateKnittingProjectTimerDto
        {
            ElapsedSeconds = request.ElapsedSeconds
        }, userId);

        return project == null ? null : Map(project);
    }

    public async Task<ModuleProjectDto?> SetTimerAsync(Guid projectId, Guid patternStepId, UpdateModuleProjectTimerRequest request, Guid userId)
    {
        var project = await _service.SetTimerAsync(projectId, patternStepId, new UpdateKnittingProjectTimerDto
        {
            ElapsedSeconds = request.ElapsedSeconds
        }, userId);

        return project == null ? null : Map(project);
    }

    public async Task<ModuleProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId)
    {
        var project = await _service.ResetTimerAsync(projectId, patternStepId, userId);
        return project == null ? null : Map(project);
    }

    public async Task<ModuleProjectDto?> AddInventoryLinkAsync(Guid projectId, CreateModuleProjectInventoryLinkRequest request, Guid userId)
    {
        var project = await _service.AddInventoryLinkAsync(projectId, new CreateKnittingProjectInventoryLinkDto
        {
            SupplyItemId = request.SupplyItemId,
            QuantityPlanned = request.QuantityPlanned,
            Notes = request.Notes
        }, userId);

        return project == null ? null : Map(project);
    }

    public Task<bool> RemoveInventoryLinkAsync(Guid projectId, Guid linkId, Guid userId)
        => _service.RemoveInventoryLinkAsync(projectId, linkId, userId);

    private static ModuleProjectDto Map(KnittingProjectDto source)
    {
        return new ModuleProjectDto
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            Description = source.Description,
            PatternId = source.PatternId,
            PatternName = source.PatternName,
            ThemeId = source.ThemeId,
            ThemeName = source.ThemeName,
            Difficulty = source.Difficulty,
            Progress = source.Progress,
            IsArchived = source.IsArchived,
            ArchivedAt = source.ArchivedAt,
            CompletedStepCount = source.CompletedStepCount,
            TotalStepCount = source.TotalStepCount,
            TotalTrackedSeconds = source.TotalTrackedSeconds,
            TimerRunning = source.TimerRunning,
            TimerStartedAt = source.TimerStartedAt,
            StepProgress = source.StepProgress.Select(progress => new ModuleProjectStepProgressDto
            {
                ProjectId = progress.ProjectId,
                PatternStepId = progress.PatternStepId,
                IsComplete = progress.IsComplete,
                CompletedAt = progress.CompletedAt
            }).ToList(),
            Timers = source.Timers.Select(timer => new ModuleProjectTimerDto
            {
                Id = timer.Id,
                ProjectId = timer.ProjectId,
                PatternStepId = timer.PatternStepId,
                ElapsedSeconds = timer.ElapsedSeconds,
                IsRunning = timer.IsRunning,
                StartedAt = timer.StartedAt,
                CreatedAt = timer.CreatedAt,
                UpdatedAt = timer.UpdatedAt
            }).ToList(),
            InventoryLinks = source.InventoryLinks.Select(link => new ModuleProjectInventoryLinkDto
            {
                Id = link.Id,
                ProjectId = link.ProjectId,
                SupplyItemId = link.SupplyItemId,
                SupplyItemName = link.SupplyItemName,
                QuantityPlanned = link.QuantityPlanned,
                Notes = link.Notes,
                CreatedAt = link.CreatedAt,
                UpdatedAt = link.UpdatedAt
            }).ToList(),
            UserId = source.UserId,
            Username = source.Username,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
