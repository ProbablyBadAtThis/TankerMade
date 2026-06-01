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
            UserId = source.UserId,
            Username = source.Username,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
