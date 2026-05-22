using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Enums;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.Crafting;

public class CraftingProjectService : ICraftingProjectService
{
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

    public async Task<IReadOnlyList<CraftingProjectDto>> GetAllAsync(Guid userId)
    {
        var projects = await _context.CraftingProjects
            .Where(p => p.UserId == userId)
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

        project.Update(
            UseIncomingValue(updateDto.Name, project.Name),
            UseIncomingValue(updateDto.Description, project.Description),
            patternId,
            updateDto.ThemeId ?? project.ThemeId,
            updateDto.Difficulty ?? project.Difficulty,
            updateDto.Progress ?? project.Progress);
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
            .Where(p => p.UserId == userId && (p.Name.Contains(normalized) || p.Description.Contains(normalized)))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return await MapListAsync(projects);
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
            Difficulty = project.Difficulty,
            DifficultyLabel = GetDifficultyLabel(project.Difficulty),
            Progress = project.Progress,
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

    private static string UseIncomingValue(string? incomingValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(incomingValue)
            ? currentValue
            : incomingValue.Trim();
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
}
