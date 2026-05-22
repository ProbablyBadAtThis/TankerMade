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
    public string DisplayRange { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
