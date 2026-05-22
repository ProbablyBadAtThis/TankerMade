using System.Net;
using System.Net.Http.Json;
using TankerMade.Contracts.DTOs.Modules;

namespace TankerMade.Client.Services;

public class TankerMadeApiClient
{
    private readonly HttpClient _http;

    public TankerMadeApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ModuleDto>> GetModulesAsync()
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<ModuleDto>>("api/modules") ?? [];
    }

    public async Task<ModuleDto?> ActivateModuleAsync(string moduleKey)
    {
        var response = await _http.PostAsJsonAsync("api/modules/activate", new ModuleActivationRequest
        {
            ModuleKey = moduleKey
        });

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ModuleDto>();
    }

    public async Task DeactivateModuleAsync(string moduleKey)
    {
        var response = await _http.PostAsJsonAsync("api/modules/deactivate", new ModuleActivationRequest
        {
            ModuleKey = moduleKey
        });

        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<IReadOnlyList<CraftingPatternSummary>> GetCraftingPatternsAsync()
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingPatternSummary>>("api/modules/crafting/patterns") ?? [];
    }

    public async Task<IReadOnlyList<CraftingProjectSummary>> GetCraftingProjectsAsync()
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingProjectSummary>>("api/modules/crafting/projects") ?? [];
    }

    public async Task<CraftingProjectSummary?> GetCraftingProjectAsync(Guid projectId)
    {
        var response = await _http.GetAsync($"api/modules/crafting/projects/{projectId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary> CreateCraftingProjectAsync(CraftingProjectFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/projects", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>()
            ?? throw new InvalidOperationException("The server returned an empty project response.");
    }

    public async Task<CraftingProjectSummary?> UpdateCraftingProjectAsync(Guid projectId, CraftingProjectFormRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/projects/{projectId}", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> SetCraftingProjectStepProgressAsync(
        Guid projectId,
        Guid patternStepId,
        bool isComplete)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/projects/{projectId}/steps/{patternStepId}/progress",
            new CraftingProjectStepProgressRequest { IsComplete = isComplete });

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingPatternDetail?> GetCraftingPatternAsync(Guid patternId)
    {
        var response = await _http.GetAsync($"api/modules/crafting/patterns/{patternId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternDetail>();
    }

    public async Task<CraftingPatternDetail> CreateCraftingPatternAsync(CraftingPatternFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/patterns", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CraftingPatternDetail>()
            ?? throw new InvalidOperationException("The server returned an empty pattern response.");
    }

    public async Task<CraftingPatternDetail?> UpdateCraftingPatternAsync(Guid patternId, CraftingPatternFormRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/patterns/{patternId}", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternDetail>();
    }

    public async Task<CraftingPatternPieceDetail?> AddCraftingPatternPieceAsync(Guid patternId, CraftingPatternPieceFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/patterns/{patternId}/pieces", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternPieceDetail>();
    }

    public async Task<CraftingPatternPieceDetail?> UpdateCraftingPatternPieceAsync(
        Guid patternId,
        Guid pieceId,
        CraftingPatternPieceFormRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternPieceDetail>();
    }

    public async Task<bool> DeleteCraftingPatternPieceAsync(Guid patternId, Guid pieceId)
    {
        var response = await _http.DeleteAsync($"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ReorderCraftingPatternPiecesAsync(Guid patternId, IReadOnlyList<Guid> orderedIds)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/patterns/{patternId}/pieces/reorder",
            new ReorderCraftingPatternItemsRequest { OrderedIds = orderedIds });

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<CraftingPatternStepDetail?> AddCraftingPatternStepAsync(
        Guid patternId,
        Guid pieceId,
        CraftingPatternStepFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}/steps", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternStepDetail>();
    }

    public async Task<CraftingPatternStepDetail?> UpdateCraftingPatternStepAsync(
        Guid patternId,
        Guid pieceId,
        Guid stepId,
        CraftingPatternStepFormRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}/steps/{stepId}", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingPatternStepDetail>();
    }

    public async Task<bool> DeleteCraftingPatternStepAsync(Guid patternId, Guid pieceId, Guid stepId)
    {
        var response = await _http.DeleteAsync($"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}/steps/{stepId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ReorderCraftingPatternStepsAsync(Guid patternId, Guid pieceId, IReadOnlyList<Guid> orderedIds)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/patterns/{patternId}/pieces/{pieceId}/steps/reorder",
            new ReorderCraftingPatternItemsRequest { OrderedIds = orderedIds });

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}

public class CraftingPatternFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public Guid? SourceId { get; set; }
}

public class CraftingPatternPieceFormRequest
{
    public string Name { get; set; } = string.Empty;
}

public class CraftingPatternStepFormRequest
{
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
}

public class ReorderCraftingPatternItemsRequest
{
    public IReadOnlyList<Guid> OrderedIds { get; set; } = [];
}

public class CraftingProjectFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public bool ClearPatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public int Difficulty { get; set; }
    public int Progress { get; set; }
}

public class CraftingProjectStepProgressRequest
{
    public bool IsComplete { get; set; }
}

public class CraftingProjectSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string PatternName { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int CompletedStepCount { get; set; }
    public int TotalStepCount { get; set; }
    public IReadOnlyList<CraftingProjectStepProgressDetail> StepProgress { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CraftingProjectStepProgressDetail
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class CraftingPatternSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int PieceCount { get; set; }
    public int StepCount { get; set; }
}

public class CraftingPatternDetail : CraftingPatternSummary
{
    public IReadOnlyList<CraftingPatternPieceDetail> Pieces { get; set; } = [];
    public CraftingPatternProgressDetail Progress { get; set; } = new();
}

public class CraftingPatternProgressDetail
{
    public int PieceCount { get; set; }
    public int StepCount { get; set; }
    public int EmptyPieceCount { get; set; }
    public int InvalidRangeCount { get; set; }
    public bool HasPieces { get; set; }
    public bool HasSteps { get; set; }
    public bool IsReadyForProject { get; set; }
    public IReadOnlyList<string> ValidationMessages { get; set; } = [];
}

public class CraftingPatternPieceDetail
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public IReadOnlyList<CraftingPatternStepDetail> Steps { get; set; } = [];
}

public class CraftingPatternStepDetail
{
    public Guid Id { get; set; }
    public int? RangeStart { get; set; }
    public int? RangeEnd { get; set; }
    public string DisplayRange { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
