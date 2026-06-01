using Microsoft.EntityFrameworkCore;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Knitting;

public class KnittingProjectService : IKnittingProjectService
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;
    private readonly TankerMadeDbContext _context;

    public KnittingProjectService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<KnittingProjectDto> CreateAsync(CreateKnittingProjectDto createDto, Guid userId)
    {
        await EnsurePatternBelongsToUserAsync(createDto.PatternId, userId);

        var project = new KnittingProject(Guid.NewGuid(), createDto.Name, userId);
        project.Update(
            createDto.Name,
            createDto.Description,
            createDto.PatternId,
            createDto.ThemeId,
            createDto.Difficulty,
            createDto.Progress ?? 0);

        _context.KnittingProjects.Add(project);
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<IReadOnlyList<KnittingProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false, int page = 1, int pageSize = DefaultPageSize)
    {
        var (skip, take) = ResolvePaging(page, pageSize);
        var projects = await _context.KnittingProjects
            .Where(project => project.UserId == userId && (includeArchived || !project.IsArchived))
            .OrderBy(project => project.IsArchived)
            .ThenBy(project => project.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return await MapListAsync(projects);
    }

    public async Task<IReadOnlyList<KnittingProjectDto>> SearchAsync(string query, Guid userId, int page = 1, int pageSize = DefaultPageSize)
    {
        var terms = query
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(term => term.ToLowerInvariant())
            .Distinct()
            .ToList();
        if (terms.Count == 0)
        {
            return [];
        }

        var projectQuery = _context.KnittingProjects.Where(project => project.UserId == userId);
        foreach (var term in terms)
        {
            var like = $"%{term}%";
            projectQuery = projectQuery.Where(project =>
                EF.Functions.Like((project.Name ?? string.Empty).ToLower(), like)
                || EF.Functions.Like((project.Description ?? string.Empty).ToLower(), like));
        }

        var (skip, take) = ResolvePaging(page, pageSize);
        var projects = await projectQuery
            .OrderBy(project => project.IsArchived)
            .ThenBy(project => project.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return await MapListAsync(projects);
    }

    public async Task<KnittingProjectDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == id && project.UserId == userId);
        return project == null ? null : await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> UpdateAsync(UpdateKnittingProjectDto updateDto, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == updateDto.Id && project.UserId == userId);
        if (project == null)
        {
            return null;
        }

        var patternId = updateDto.ClearPatternId ? null : updateDto.PatternId ?? project.PatternId;
        await EnsurePatternBelongsToUserAsync(patternId, userId);

        project.Update(
            UseIncomingValue(updateDto.Name, project.Name),
            updateDto.Description ?? project.Description,
            patternId,
            updateDto.ThemeId ?? project.ThemeId,
            updateDto.Difficulty ?? project.Difficulty,
            updateDto.Progress ?? project.Progress);

        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> ArchiveAsync(Guid id, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == id && project.UserId == userId);
        if (project == null)
        {
            return null;
        }

        project.Archive(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> ReopenAsync(Guid id, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == id && project.UserId == userId);
        if (project == null)
        {
            return null;
        }

        project.Reopen(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(project => project.Id == id && project.UserId == userId);
        if (project == null)
        {
            return false;
        }

        _context.KnittingProjects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<KnittingProjectDto> MapAsync(KnittingProject project)
    {
        var userName = await _context.Users
            .Where(user => user.Id == project.UserId)
            .Select(user => user.Username)
            .SingleOrDefaultAsync() ?? string.Empty;

        var patternName = project.PatternId == null
            ? string.Empty
            : await _context.KnittingPatterns
                .Where(pattern => pattern.Id == project.PatternId)
                .Select(pattern => pattern.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var themeName = project.ThemeId == null
            ? string.Empty
            : await _context.Themes
                .Where(theme => theme.Id == project.ThemeId)
                .Select(theme => theme.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        return new KnittingProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            Description = project.Description,
            PatternId = project.PatternId,
            PatternName = patternName,
            ThemeId = project.ThemeId,
            ThemeName = themeName,
            Difficulty = project.Difficulty,
            Progress = project.Progress,
            IsArchived = project.IsArchived,
            ArchivedAt = project.ArchivedAt,
            UserId = project.UserId,
            Username = userName,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    private async Task<IReadOnlyList<KnittingProjectDto>> MapListAsync(IReadOnlyList<KnittingProject> projects)
    {
        var result = new List<KnittingProjectDto>(projects.Count);
        foreach (var project in projects)
        {
            result.Add(await MapAsync(project));
        }

        return result;
    }

    private async Task EnsurePatternBelongsToUserAsync(Guid? patternId, Guid userId)
    {
        if (patternId == null)
        {
            return;
        }

        var exists = await _context.KnittingPatterns.AnyAsync(pattern => pattern.Id == patternId && pattern.UserId == userId);
        if (!exists)
        {
            throw new InvalidOperationException("The selected pattern is not available for this project.");
        }
    }

    private static string UseIncomingValue(string? incomingValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(incomingValue)
            ? currentValue
            : incomingValue.Trim();
    }

    private static (int Skip, int Take) ResolvePaging(int page, int pageSize)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return ((safePage - 1) * safePageSize, safePageSize);
    }
}
