using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Enums;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Crafting;

public class CraftingProjectService : ICraftingProjectService
{
    private const string YarnInventoryType = "yarn";
    private const string ToolInventoryType = "tool";
    private const string NotionInventoryType = "notion";
    private readonly TankerMadeDbContext _context;

    public CraftingProjectService(TankerMadeDbContext context)
    {
        _context = context;
    }

    public async Task<CraftingProjectDto> CreateAsync(CreateCraftingProjectDto createDto, Guid userId)
    {
        await EnsurePatternBelongsToUserAsync(createDto.PatternId, userId);

        var project = new CraftingProject(Guid.NewGuid(), createDto.Name, userId);
        project.Update(createDto.Name, createDto.Description, createDto.PatternId, createDto.ThemeId, createDto.Difficulty, createDto.Progress);

        _context.CraftingProjects.Add(project);
        project.SetProgress(await CalculateStepCompletionPercentAsync(project));
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        return project == null ? null : await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> GetBySlugAsync(string slug, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Slug == slug && p.UserId == userId);

        return project == null ? null : await MapAsync(project);
    }

    public async Task<IReadOnlyList<CraftingProjectDto>> GetAllAsync(Guid userId, bool includeArchived = false)
    {
        var projects = await _context.CraftingProjects
            .Where(p => p.UserId == userId && (includeArchived || !p.IsArchived))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(projects);
    }

    public async Task<CraftingProjectDto?> UpdateAsync(UpdateCraftingProjectDto updateDto, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == updateDto.Id && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        var patternId = updateDto.ClearPatternId ? null : updateDto.PatternId ?? project.PatternId;
        await EnsurePatternBelongsToUserAsync(patternId, userId);
        if (patternId != project.PatternId && await HasWorkspaceStateAsync(project.Id))
        {
            throw new InvalidOperationException("This project already has progress or timer history. Reopen it without changing the linked pattern, or create a new project for a different pattern.");
        }

        project.Update(
            UseIncomingValue(updateDto.Name, project.Name),
            UseIncomingValue(updateDto.Description, project.Description),
            patternId,
            updateDto.ThemeId ?? project.ThemeId,
            updateDto.Difficulty ?? project.Difficulty,
            updateDto.Progress ?? project.Progress);
        project.SetProgress(await CalculateStepCompletionPercentAsync(project));
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> ArchiveAsync(Guid id, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        project.Archive(DateTime.UtcNow);
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> ReopenAsync(Guid id, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

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
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (project == null)
        {
            return false;
        }

        _context.CraftingProjects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<CraftingProjectDto>> SearchAsync(string searchTerm, Guid userId)
    {
        var normalized = searchTerm.Trim();
        var projects = await _context.CraftingProjects
            .Where(p => p.UserId == userId && !p.IsArchived && (p.Name.Contains(normalized) || p.Description.Contains(normalized)))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(projects);
    }

    public async Task<CraftingProjectDto?> SetStepProgressAsync(
        Guid projectId,
        Guid patternStepId,
        UpdateCraftingProjectStepProgressDto updateDto,
        Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var existing = await _context.CraftingProjectStepProgress
            .SingleOrDefaultAsync(progress => progress.ProjectId == projectId && progress.PatternStepId == patternStepId);

        if (updateDto.IsComplete)
        {
            if (existing == null)
            {
                _context.CraftingProjectStepProgress.Add(new CraftingProjectStepProgress(Guid.NewGuid(), projectId, patternStepId));
            }
            else if (!existing.IsComplete)
            {
                existing.Complete();
            }
        }
        else if (existing != null)
        {
            _context.CraftingProjectStepProgress.Remove(existing);
        }

        project.SetProgress(await CalculateStepCompletionPercentAsync(project));
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> StartTimerAsync(
        Guid projectId,
        Guid patternStepId,
        UpdateCraftingProjectTimerDto updateDto,
        Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var now = DateTime.UtcNow;
        var timers = await _context.CraftingProjectTimers
            .Where(timer => timer.ProjectId == projectId)
            .ToListAsync();

        foreach (var runningTimer in timers.Where(timer => timer.IsRunning))
        {
            runningTimer.Pause(now);
        }

        var timer = timers.SingleOrDefault(existing => existing.PatternStepId == patternStepId);
        if (timer == null)
        {
            timer = new CraftingProjectTimer(Guid.NewGuid(), projectId, patternStepId);
            _context.CraftingProjectTimers.Add(timer);
        }

        timer.Start(now);
        await _context.SaveChangesAsync();

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> PauseTimerAsync(
        Guid projectId,
        Guid patternStepId,
        UpdateCraftingProjectTimerDto updateDto,
        Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        await EnsureStepBelongsToLinkedPatternAsync(project, patternStepId);

        var timer = await _context.CraftingProjectTimers
            .SingleOrDefaultAsync(existing => existing.ProjectId == projectId && existing.PatternStepId == patternStepId);

        if (timer != null)
        {
            timer.Pause(DateTime.UtcNow);
            await _context.SaveChangesAsync();
        }

        return await MapAsync(project);
    }

    public async Task<CraftingProjectDto?> SetTimerAsync(
        Guid projectId,
        Guid patternStepId,
        UpdateCraftingProjectTimerDto updateDto,
        Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

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

    public async Task<CraftingProjectDto?> ResetTimerAsync(Guid projectId, Guid patternStepId, Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

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

    public async Task<CraftingProjectDto?> AddInventoryLinkAsync(
        Guid projectId,
        CreateCraftingProjectInventoryLinkDto createDto,
        Guid userId)
    {
        var project = await _context.CraftingProjects
            .SingleOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
        {
            return null;
        }

        var itemType = NormalizeInventoryItemType(createDto.InventoryItemType);
        await EnsureInventoryItemBelongsToUserAsync(itemType, createDto.InventoryItemId, userId);

        var existing = await _context.CraftingProjectInventoryLinks
            .SingleOrDefaultAsync(link => link.ProjectId == projectId
                && link.InventoryItemType == itemType
                && link.InventoryItemId == createDto.InventoryItemId);

        if (existing == null)
        {
            _context.CraftingProjectInventoryLinks.Add(new CraftingProjectInventoryLink(
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
        var projectExists = await _context.CraftingProjects
            .AnyAsync(p => p.Id == projectId && p.UserId == userId);

        if (!projectExists)
        {
            return false;
        }

        var link = await _context.CraftingProjectInventoryLinks
            .SingleOrDefaultAsync(existing => existing.Id == linkId && existing.ProjectId == projectId);

        if (link == null)
        {
            return false;
        }

        _context.CraftingProjectInventoryLinks.Remove(link);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<CraftingProjectDto> MapAsync(CraftingProject project)
    {
        var userName = await _context.Users
            .Where(u => u.Id == project.UserId)
            .Select(u => u.Username)
            .SingleOrDefaultAsync() ?? string.Empty;

        var patternName = project.PatternId == null
            ? string.Empty
            : await _context.CraftingPatterns
                .Where(p => p.Id == project.PatternId)
                .Select(p => p.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var themeName = project.ThemeId == null
            ? string.Empty
            : await _context.Themes
                .Where(t => t.Id == project.ThemeId)
                .Select(t => t.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var kitName = project.KitId == null
            ? string.Empty
            : await _context.CraftingKits
                .Where(kit => kit.Id == project.KitId.Value)
                .Select(kit => kit.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var kitPieceName = project.KitPieceId == null
            ? string.Empty
            : await _context.CraftingKitPieces
                .Where(piece => piece.Id == project.KitPieceId.Value)
                .Select(piece => piece.Name)
                .SingleOrDefaultAsync() ?? string.Empty;

        var patternStepIds = await GetLinkedPatternStepIdsAsync(project);
        var stepProgress = new List<CraftingProjectStepProgressDto>();
        if (patternStepIds.Count > 0)
        {
            var patternStepIdSet = patternStepIds.ToHashSet();
            var completedProgress = await _context.CraftingProjectStepProgress
                .Where(progress => progress.ProjectId == project.Id && progress.IsComplete)
                .OrderBy(progress => progress.CompletedAt)
                .ToListAsync();

            stepProgress = completedProgress
                .Where(progress => patternStepIdSet.Contains(progress.PatternStepId))
                .Select(progress => new CraftingProjectStepProgressDto
                {
                    ProjectId = progress.ProjectId,
                    PatternStepId = progress.PatternStepId,
                    IsComplete = progress.IsComplete,
                    CompletedAt = progress.CompletedAt
                })
                .ToList();
        }

        var now = DateTime.UtcNow;
        var timers = await _context.CraftingProjectTimers
            .Where(timer => timer.ProjectId == project.Id)
            .OrderBy(timer => timer.CreatedAt)
            .ToListAsync();

        var timerDtos = timers
            .Select(timer => new CraftingProjectTimerDto
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

        return new CraftingProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            Description = project.Description,
            PatternId = project.PatternId,
            PatternName = patternName,
            ThemeId = project.ThemeId,
            ThemeName = themeName,
            KitId = project.KitId,
            KitName = kitName,
            KitPieceId = project.KitPieceId,
            KitPieceName = kitPieceName,
            Difficulty = project.Difficulty,
            DifficultyLabel = GetDifficultyLabel(project.Difficulty),
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

    private async Task<IReadOnlyList<CraftingProjectDto>> MapListAsync(IReadOnlyList<CraftingProject> projects)
    {
        var results = new List<CraftingProjectDto>(projects.Count);
        foreach (var project in projects)
        {
            results.Add(await MapAsync(project));
        }

        return results;
    }

    private static string GetDifficultyLabel(int difficulty)
    {
        return Enum.IsDefined(typeof(Difficulty), difficulty)
            ? ((Difficulty)difficulty).ToString()
            : string.Empty;
    }

    private async Task<int> CalculateStepCompletionPercentAsync(CraftingProject project)
    {
        var patternStepIds = await GetLinkedPatternStepIdsAsync(project);
        if (patternStepIds.Count == 0)
        {
            return project.Progress;
        }

        var completedCount = await _context.CraftingProjectStepProgress
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

    private static string UseIncomingValue(string? incomingValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(incomingValue)
            ? currentValue
            : incomingValue.Trim();
    }

    private async Task<CraftingProjectTimer> GetOrCreateTimerAsync(Guid projectId, Guid patternStepId)
    {
        var timer = await _context.CraftingProjectTimers
            .SingleOrDefaultAsync(existing => existing.ProjectId == projectId && existing.PatternStepId == patternStepId);

        if (timer != null)
        {
            return timer;
        }

        timer = new CraftingProjectTimer(Guid.NewGuid(), projectId, patternStepId);
        _context.CraftingProjectTimers.Add(timer);
        return timer;
    }

    private async Task<bool> HasWorkspaceStateAsync(Guid projectId)
    {
        return await _context.CraftingProjectStepProgress.AnyAsync(progress => progress.ProjectId == projectId)
            || await _context.CraftingProjectTimers.AnyAsync(timer => timer.ProjectId == projectId);
    }

    private async Task<IReadOnlyList<CraftingProjectInventoryLinkDto>> MapInventoryLinksAsync(Guid projectId)
    {
        var links = await _context.CraftingProjectInventoryLinks
            .Where(link => link.ProjectId == projectId)
            .OrderBy(link => link.InventoryItemType)
            .ThenBy(link => link.CreatedAt)
            .ToListAsync();

        var results = new List<CraftingProjectInventoryLinkDto>(links.Count);
        foreach (var link in links)
        {
            results.Add(new CraftingProjectInventoryLinkDto
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

    private async Task<string> GetInventoryItemNameAsync(string itemType, Guid itemId)
    {
        return itemType switch
        {
            YarnInventoryType => await _context.CraftingYarnInventoryItems
                .Where(item => item.Id == itemId)
                .Select(item => item.BrandName + " - " + item.ColorName)
                .SingleOrDefaultAsync() ?? string.Empty,
            ToolInventoryType => await _context.CraftingToolInventoryItems
                .Where(item => item.Id == itemId)
                .Select(item => item.BrandName + " - " + item.TypeName)
                .SingleOrDefaultAsync() ?? string.Empty,
            NotionInventoryType => await _context.CraftingNotionInventoryItems
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
            YarnInventoryType => await _context.CraftingYarnInventoryItems
                .AnyAsync(item => item.Id == itemId && item.UserId == userId),
            ToolInventoryType => await _context.CraftingToolInventoryItems
                .AnyAsync(item => item.Id == itemId && item.UserId == userId),
            NotionInventoryType => await _context.CraftingNotionInventoryItems
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

    private async Task EnsurePatternBelongsToUserAsync(Guid? patternId, Guid userId)
    {
        if (patternId == null)
        {
            return;
        }

        var exists = await _context.CraftingPatterns
            .AnyAsync(pattern => pattern.Id == patternId && pattern.UserId == userId);

        if (!exists)
        {
            throw new InvalidOperationException("The selected pattern is not available for this project.");
        }
    }

    private async Task EnsureStepBelongsToLinkedPatternAsync(CraftingProject project, Guid patternStepId)
    {
        if (project.PatternId == null)
        {
            throw new InvalidOperationException("Link a pattern before tracking step progress.");
        }

        var belongsToPattern = await _context.CraftingPatternSteps
            .Join(
                _context.CraftingPatternPieces,
                step => step.PatternPieceId,
                piece => piece.Id,
                (step, piece) => new { step.Id, piece.PatternId })
            .AnyAsync(step => step.Id == patternStepId && step.PatternId == project.PatternId);

        if (!belongsToPattern)
        {
            throw new InvalidOperationException("The selected step is not available for this project.");
        }
    }

    private async Task<IReadOnlyList<Guid>> GetLinkedPatternStepIdsAsync(CraftingProject project)
    {
        if (project.PatternId == null)
        {
            return [];
        }

        return await _context.CraftingPatternSteps
            .Join(
                _context.CraftingPatternPieces,
                step => step.PatternPieceId,
                piece => piece.Id,
                (step, piece) => new { step.Id, piece.PatternId })
            .Where(step => step.PatternId == project.PatternId)
            .Select(step => step.Id)
            .ToListAsync();
    }
}
