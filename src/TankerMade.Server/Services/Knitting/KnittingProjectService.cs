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
        project.SetProgress(await CalculateStepCompletionPercentAsync(project));
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
        if (patternId != project.PatternId && await HasWorkspaceStateAsync(project.Id))
        {
            throw new InvalidOperationException("This project already has progress or timer history. Keep the linked pattern or create a new project.");
        }

        project.Update(
            UseIncomingValue(updateDto.Name, project.Name),
            updateDto.Description == null
                ? project.Description
                : updateDto.Description.Trim(),
            patternId,
            updateDto.ThemeId ?? project.ThemeId,
            updateDto.Difficulty ?? project.Difficulty,
            updateDto.Progress ?? project.Progress);
        project.SetProgress(await CalculateStepCompletionPercentAsync(project));

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

    public async Task<KnittingProjectDto?> SetStepProgressAsync(
        Guid projectId,
        Guid patternStepId,
        UpdateKnittingProjectStepProgressDto updateDto,
        Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var existing = await _context.KnittingProjectStepProgress
            .SingleOrDefaultAsync(progress => progress.ProjectId == projectId && progress.PatternStepId == patternStepId);

        if (updateDto.IsComplete)
        {
            if (existing == null)
            {
                _context.KnittingProjectStepProgress.Add(new KnittingProjectStepProgress(Guid.NewGuid(), projectId, patternStepId));
            }
            else if (!existing.IsComplete)
            {
                existing.Complete();
            }
        }
        else if (existing != null)
        {
            _context.KnittingProjectStepProgress.Remove(existing);
        }

        project.SetProgress(await CalculateStepCompletionPercentAsync(project));
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> StartTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var now = DateTime.UtcNow;
        var timers = await _context.KnittingProjectTimers
            .Where(timer => timer.ProjectId == projectId)
            .ToListAsync();

        foreach (var runningTimer in timers.Where(timer => timer.IsRunning))
        {
            runningTimer.Pause(now);
        }

        var timer = timers.SingleOrDefault(existing => existing.PatternStepId == patternStepId);
        if (timer == null)
        {
            timer = new KnittingProjectTimer(Guid.NewGuid(), projectId, patternStepId);
            _context.KnittingProjectTimers.Add(timer);
        }

        timer.Start(now);
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> PauseTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var timer = await _context.KnittingProjectTimers
            .SingleOrDefaultAsync(existing => existing.ProjectId == projectId && existing.PatternStepId == patternStepId);

        if (timer != null)
        {
            timer.Pause(DateTime.UtcNow);
            await _context.SaveChangesAsync();
        }

        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> SetTimerAsync(Guid projectId, Guid patternStepId, UpdateKnittingProjectTimerDto updateDto, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        if (updateDto.ElapsedSeconds.HasValue && updateDto.ElapsedSeconds.Value < 0)
        {
            throw new ArgumentException("Elapsed time cannot be negative.", nameof(updateDto));
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);
        var timer = await GetOrCreateTimerAsync(projectId, patternStepId);
        timer.SetElapsedSeconds(updateDto.ElapsedSeconds ?? 0, DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);
        var timer = await GetOrCreateTimerAsync(projectId, patternStepId);
        timer.Reset(DateTime.UtcNow);
        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<KnittingProjectDto?> AddInventoryLinkAsync(Guid projectId, CreateKnittingProjectInventoryLinkDto createDto, Guid userId)
    {
        var project = await _context.KnittingProjects.SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        if (project == null)
        {
            return null;
        }

        var itemType = NormalizeInventoryItemType(createDto.InventoryItemType);
        await EnsureInventoryItemBelongsToUserAsync(itemType, createDto.InventoryItemId, userId);

        var existing = await _context.KnittingProjectInventoryLinks
            .SingleOrDefaultAsync(link => link.ProjectId == projectId
                && link.InventoryItemType == itemType
                && link.InventoryItemId == createDto.InventoryItemId);

        if (existing == null)
        {
            _context.KnittingProjectInventoryLinks.Add(new KnittingProjectInventoryLink(
                Guid.NewGuid(),
                projectId,
                itemType,
                createDto.InventoryItemId,
                createDto.QuantityPlanned,
                createDto.Notes));
        }
        else
        {
            existing.Update(createDto.QuantityPlanned, createDto.Notes);
        }

        await _context.SaveChangesAsync();
        return await MapAsync(project);
    }

    public async Task<bool> RemoveInventoryLinkAsync(Guid projectId, Guid linkId, Guid userId)
    {
        var projectExists = await _context.KnittingProjects.AnyAsync(p => p.Id == projectId && p.UserId == userId);
        if (!projectExists)
        {
            return false;
        }

        var link = await _context.KnittingProjectInventoryLinks
            .SingleOrDefaultAsync(existing => existing.Id == linkId && existing.ProjectId == projectId);

        if (link == null)
        {
            return false;
        }

        _context.KnittingProjectInventoryLinks.Remove(link);
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

        var patternStepIds = await GetLinkedPatternStepIdsAsync(project);
        var stepProgress = new List<KnittingProjectStepProgressDto>();
        if (patternStepIds.Count > 0)
        {
            var stepIdSet = patternStepIds.ToHashSet();
            var completed = await _context.KnittingProjectStepProgress
                .Where(progress => progress.ProjectId == project.Id && progress.IsComplete)
                .OrderBy(progress => progress.CompletedAt)
                .ToListAsync();

            stepProgress = completed
                .Where(progress => stepIdSet.Contains(progress.PatternStepId))
                .Select(progress => new KnittingProjectStepProgressDto
                {
                    ProjectId = progress.ProjectId,
                    PatternStepId = progress.PatternStepId,
                    IsComplete = progress.IsComplete,
                    CompletedAt = progress.CompletedAt
                })
                .ToList();
        }

        var now = DateTime.UtcNow;
        var timers = await _context.KnittingProjectTimers
            .Where(timer => timer.ProjectId == project.Id)
            .OrderBy(timer => timer.CreatedAt)
            .ToListAsync();

        var timerDtos = timers
            .Select(timer => new KnittingProjectTimerDto
            {
                Id = timer.Id,
                ProjectId = timer.ProjectId,
                PatternStepId = timer.PatternStepId,
                ElapsedSeconds = timer.ElapsedSeconds,
                IsRunning = timer.IsRunning,
                StartedAt = timer.StartedAt,
                CreatedAt = timer.CreatedAt,
                UpdatedAt = timer.UpdatedAt
            })
            .ToList();

        var inventoryLinks = await MapInventoryLinksAsync(project.Id);

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
            Progress = patternStepIds.Count == 0
                ? project.Progress
                : CalculateCompletionPercent(stepProgress.Count, patternStepIds.Count),
            IsArchived = project.IsArchived,
            ArchivedAt = project.ArchivedAt,
            CompletedStepCount = stepProgress.Count,
            TotalStepCount = patternStepIds.Count,
            TotalTrackedSeconds = timers.Sum(timer => timer.GetElapsedSeconds(now)),
            TimerRunning = timerDtos.Any(timer => timer.IsRunning),
            TimerStartedAt = timerDtos.FirstOrDefault(timer => timer.IsRunning)?.StartedAt,
            StepProgress = stepProgress,
            Timers = timerDtos,
            InventoryLinks = inventoryLinks,
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

    private async Task<KnittingProjectTimer> GetOrCreateTimerAsync(Guid projectId, Guid patternStepId)
    {
        var timer = await _context.KnittingProjectTimers
            .SingleOrDefaultAsync(existing => existing.ProjectId == projectId && existing.PatternStepId == patternStepId);

        if (timer != null)
        {
            return timer;
        }

        timer = new KnittingProjectTimer(Guid.NewGuid(), projectId, patternStepId);
        _context.KnittingProjectTimers.Add(timer);
        return timer;
    }

    private async Task<IReadOnlyList<KnittingProjectInventoryLinkDto>> MapInventoryLinksAsync(Guid projectId)
    {
        var links = await _context.KnittingProjectInventoryLinks
            .Where(link => link.ProjectId == projectId)
            .OrderBy(link => link.CreatedAt)
            .ToListAsync();

        var results = new List<KnittingProjectInventoryLinkDto>(links.Count);
        foreach (var link in links)
        {
            results.Add(new KnittingProjectInventoryLinkDto
            {
                Id = link.Id,
                ProjectId = link.ProjectId,
                InventoryItemType = link.InventoryItemType,
                InventoryItemId = link.InventoryItemId,
                InventoryItemName = await GetInventoryItemNameAsync(link.InventoryItemType, link.InventoryItemId),
                QuantityPlanned = link.QuantityPlanned,
                Notes = link.Notes,
                CreatedAt = link.CreatedAt,
                UpdatedAt = link.UpdatedAt
            });
        }

        return results;
    }

    private const string YarnInventoryType = "yarn";
    private const string ToolInventoryType = "tool";
    private const string NotionInventoryType = "notion";

    private async Task<string> GetInventoryItemNameAsync(string itemType, Guid itemId)
    {
        return itemType switch
        {
            YarnInventoryType => await _context.KnittingYarnInventoryItems
                .Where(item => item.Id == itemId)
                .Select(item => item.BrandName + " - " + item.ColorName)
                .SingleOrDefaultAsync() ?? string.Empty,
            ToolInventoryType => await _context.KnittingToolInventoryItems
                .Where(item => item.Id == itemId)
                .Select(item => item.BrandName + " - " + item.TypeName)
                .SingleOrDefaultAsync() ?? string.Empty,
            NotionInventoryType => await _context.KnittingNotionInventoryItems
                .Where(item => item.Id == itemId)
                .Select(item => item.BrandName + " - " + item.TypeName)
                .SingleOrDefaultAsync() ?? string.Empty,
            _ => string.Empty
        };
    }

    private async Task EnsureInventoryItemBelongsToUserAsync(string itemType, Guid itemId, Guid userId)
    {
        var exists = itemType switch
        {
            YarnInventoryType => await _context.KnittingYarnInventoryItems
                .AnyAsync(item => item.Id == itemId && item.UserId == userId),
            ToolInventoryType => await _context.KnittingToolInventoryItems
                .AnyAsync(item => item.Id == itemId && item.UserId == userId),
            NotionInventoryType => await _context.KnittingNotionInventoryItems
                .AnyAsync(item => item.Id == itemId && item.UserId == userId),
            _ => false
        };

        if (!exists)
        {
            throw new InvalidOperationException("The selected inventory item is not available for this project.");
        }
    }

    private static string NormalizeInventoryItemType(string itemType)
    {
        var normalized = itemType.Trim().ToLowerInvariant();
        return normalized switch
        {
            YarnInventoryType => YarnInventoryType,
            ToolInventoryType => ToolInventoryType,
            NotionInventoryType => NotionInventoryType,
            _ => throw new ArgumentException("Inventory item type must be yarn, tool, or notion.", nameof(itemType))
        };
    }

    private async Task<int> CalculateStepCompletionPercentAsync(KnittingProject project)
    {
        var patternStepIds = await GetLinkedPatternStepIdsAsync(project);
        if (patternStepIds.Count == 0)
        {
            return project.Progress;
        }

        var completedCount = await _context.KnittingProjectStepProgress
            .Where(progress => progress.ProjectId == project.Id && progress.IsComplete)
            .CountAsync(progress => patternStepIds.Contains(progress.PatternStepId));

        return CalculateCompletionPercent(completedCount, patternStepIds.Count);
    }

    private static int CalculateCompletionPercent(int completedCount, int totalCount)
    {
        if (totalCount <= 0)
        {
            return 0;
        }

        var clampedCompleted = Math.Min(totalCount, Math.Max(0, completedCount));
        return (int)Math.Round(clampedCompleted * 100.0 / totalCount, MidpointRounding.AwayFromZero);
    }

    private async Task<bool> HasWorkspaceStateAsync(Guid projectId)
    {
        return await _context.KnittingProjectStepProgress.AnyAsync(progress => progress.ProjectId == projectId)
            || await _context.KnittingProjectTimers.AnyAsync(timer => timer.ProjectId == projectId)
            || await _context.KnittingProjectInventoryLinks.AnyAsync(link => link.ProjectId == projectId);
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

    private async Task EnsureStepBelongsToLinkedPatternAsync(KnittingProject project, Guid patternStepId)
    {
        if (project.PatternId == null)
        {
            throw new InvalidOperationException("Link a pattern before tracking step progress.");
        }

        var belongsToPattern = await _context.KnittingPatternSteps
            .Join(
                _context.KnittingPatternPieces,
                step => step.PatternPieceId,
                piece => piece.Id,
                (step, piece) => new { step.Id, piece.PatternId })
            .AnyAsync(step => step.Id == patternStepId && step.PatternId == project.PatternId);

        if (!belongsToPattern)
        {
            throw new InvalidOperationException("The selected step is not available for this project.");
        }
    }

    private async Task<IReadOnlyList<Guid>> GetLinkedPatternStepIdsAsync(KnittingProject project)
    {
        if (project.PatternId == null)
        {
            return [];
        }

        return await _context.KnittingPatternSteps
            .Join(
                _context.KnittingPatternPieces,
                step => step.PatternPieceId,
                piece => piece.Id,
                (step, piece) => new { step.Id, piece.PatternId })
            .Where(step => step.PatternId == project.PatternId)
            .Select(step => step.Id)
            .ToListAsync();
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
