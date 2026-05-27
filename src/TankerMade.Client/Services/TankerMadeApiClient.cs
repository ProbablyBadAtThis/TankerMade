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

    public async Task<IReadOnlyList<CraftingProjectSummary>> GetCraftingProjectsAsync(bool includeArchived = false)
    {
        var url = includeArchived
            ? "api/modules/crafting/projects?includeArchived=true"
            : "api/modules/crafting/projects";

        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingProjectSummary>>(url) ?? [];
    }

    public async Task<CraftingProjectSummary?> GetCraftingProjectAsync(Guid projectId)
    {
        var response = await _http.GetAsync($"api/modules/crafting/projects/{projectId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
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

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> AddCraftingProjectInventoryLinkAsync(
        Guid projectId,
        CraftingProjectInventoryLinkFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/projects/{projectId}/inventory-links", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<bool> RemoveCraftingProjectInventoryLinkAsync(Guid projectId, Guid linkId)
    {
        var response = await _http.DeleteAsync($"api/modules/crafting/projects/{projectId}/inventory-links/{linkId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return false;
        }

        await EnsureSuccessAsync(response);
        return true;
    }

    public async Task<CraftingProjectSummary?> ArchiveCraftingProjectAsync(Guid projectId)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/projects/{projectId}/archive", new { });
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> ReopenCraftingProjectAsync(Guid projectId)
    {
        var response = await _http.PutAsJsonAsync($"api/modules/crafting/projects/{projectId}/reopen", new { });
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

    public async Task<CraftingProjectSummary?> StartCraftingProjectTimerAsync(Guid projectId, Guid patternStepId)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/projects/{projectId}/steps/{patternStepId}/timer/start",
            new CraftingProjectTimerRequest());

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> PauseCraftingProjectTimerAsync(Guid projectId, Guid patternStepId)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/projects/{projectId}/steps/{patternStepId}/timer/pause",
            new CraftingProjectTimerRequest());

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> SetCraftingProjectTimerAsync(Guid projectId, Guid patternStepId, long elapsedSeconds)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/modules/crafting/projects/{projectId}/steps/{patternStepId}/timer",
            new CraftingProjectTimerRequest { ElapsedSeconds = elapsedSeconds });

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<CraftingProjectSummary?> ResetCraftingProjectTimerAsync(Guid projectId, Guid patternStepId)
    {
        var response = await _http.DeleteAsync($"api/modules/crafting/projects/{projectId}/steps/{patternStepId}/timer");
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

        await EnsureSuccessAsync(response);
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

        await EnsureSuccessAsync(response);
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

        await EnsureSuccessAsync(response);
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

    public async Task<IReadOnlyList<CraftingYarnInventoryItemSummary>> GetCraftingYarnsAsync(string search = "")
    {
        var url = BuildUrl("api/modules/crafting/inventory/yarns", ("search", search));
        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingYarnInventoryItemSummary>>(url) ?? [];
    }

    public async Task<CraftingYarnInventoryItemSummary> CreateCraftingYarnAsync(CraftingYarnFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/inventory/yarns", request);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<CraftingYarnInventoryItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty yarn response.");
    }

    public async Task<IReadOnlyList<CraftingToolInventoryItemSummary>> GetCraftingToolsAsync(string search = "")
    {
        var url = BuildUrl("api/modules/crafting/inventory/tools", ("search", search));
        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingToolInventoryItemSummary>>(url) ?? [];
    }

    public async Task<CraftingToolInventoryItemSummary> CreateCraftingToolAsync(CraftingToolFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/inventory/tools", request);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<CraftingToolInventoryItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty tool response.");
    }

    public async Task<IReadOnlyList<CraftingNotionInventoryItemSummary>> GetCraftingNotionsAsync(string search = "")
    {
        var url = BuildUrl("api/modules/crafting/inventory/notions", ("search", search));
        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingNotionInventoryItemSummary>>(url) ?? [];
    }

    public async Task<CraftingNotionInventoryItemSummary> CreateCraftingNotionAsync(CraftingNotionFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/inventory/notions", request);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<CraftingNotionInventoryItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty notion response.");
    }

    public async Task<IReadOnlyList<InventoryReferenceItemSummary>> GetCraftingReferenceItemsAsync(string category)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<InventoryReferenceItemSummary>>(
            $"api/modules/crafting/inventory/reference/{Uri.EscapeDataString(category)}") ?? [];
    }

    public async Task<InventoryReferenceItemSummary> CreateCraftingReferenceItemAsync(
        string category,
        string name)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/modules/crafting/inventory/reference/{Uri.EscapeDataString(category)}",
            new { name });
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<InventoryReferenceItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty reference response.");
    }

    public async Task<IReadOnlyList<PrintingMaterialInventoryItemSummary>> GetPrintingMaterialsAsync(string search = "")
    {
        var url = BuildUrl("api/modules/printing-3d/inventory/materials", ("search", search));
        return await _http.GetFromJsonAsync<IReadOnlyList<PrintingMaterialInventoryItemSummary>>(url) ?? [];
    }

    public async Task<PrintingMaterialInventoryItemSummary> CreatePrintingMaterialAsync(PrintingMaterialFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/printing-3d/inventory/materials", request);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<PrintingMaterialInventoryItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty material response.");
    }

    public async Task<IReadOnlyList<CraftingKitSummary>> GetCraftingKitsAsync(bool includeArchived = false)
    {
        var url = includeArchived
            ? "api/modules/crafting/kits?includeArchived=true"
            : "api/modules/crafting/kits";

        return await _http.GetFromJsonAsync<IReadOnlyList<CraftingKitSummary>>(url) ?? [];
    }

    public async Task<CraftingKitSummary?> GetCraftingKitAsync(Guid kitId)
    {
        var response = await _http.GetAsync($"api/modules/crafting/kits/{kitId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingKitSummary>();
    }

    public async Task<CraftingKitSummary> CreateCraftingKitAsync(CraftingKitFormRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/modules/crafting/kits", request);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<CraftingKitSummary>()
            ?? throw new InvalidOperationException("The server returned an empty kit response.");
    }

    public async Task<CraftingKitPieceSummary?> AddCraftingKitPieceAsync(Guid kitId, CraftingKitPieceFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/kits/{kitId}/pieces", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingKitPieceSummary>();
    }

    public async Task<CraftingKitSupplySummary?> AddCraftingKitSupplyAsync(Guid kitId, CraftingKitSupplyFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/kits/{kitId}/supplies", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingKitSupplySummary>();
    }

    public async Task<CraftingProjectSummary?> CreateCraftingProjectFromKitPieceAsync(
        Guid kitId,
        Guid pieceId,
        CraftingKitProjectFormRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/modules/crafting/kits/{kitId}/pieces/{pieceId}/project", request);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CraftingProjectSummary>();
    }

    public async Task<IReadOnlyList<InventoryReferenceItemSummary>> GetPrintingReferenceItemsAsync(string category)
    {
        return await _http.GetFromJsonAsync<IReadOnlyList<InventoryReferenceItemSummary>>(
            $"api/modules/printing-3d/inventory/reference/{Uri.EscapeDataString(category)}") ?? [];
    }

    public async Task<InventoryReferenceItemSummary> CreatePrintingReferenceItemAsync(
        string category,
        string name)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/modules/printing-3d/inventory/reference/{Uri.EscapeDataString(category)}",
            new { name });
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<InventoryReferenceItemSummary>()
            ?? throw new InvalidOperationException("The server returned an empty reference response.");
    }

    private static string BuildUrl(string path, params (string Name, string Value)[] parameters)
    {
        var query = parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{Uri.EscapeDataString(parameter.Name)}={Uri.EscapeDataString(parameter.Value.Trim())}");

        var queryString = string.Join("&", query);
        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(message))
        {
            response.EnsureSuccessStatusCode();
        }

        throw new InvalidOperationException(message);
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
    public int? Progress { get; set; }
}

public class CraftingProjectInventoryLinkFormRequest
{
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CraftingProjectStepProgressRequest
{
    public bool IsComplete { get; set; }
}

public class CraftingProjectTimerRequest
{
    public long? ElapsedSeconds { get; set; }
}

public class CraftingYarnFormRequest
{
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string MainColor { get; set; } = string.Empty;
    public string WeightName { get; set; } = string.Empty;
    public string FiberContent { get; set; } = string.Empty;
    public string FiberTag { get; set; } = string.Empty;
    public decimal Skeins { get; set; } = 1;
    public decimal? EstimatedLength { get; set; }
    public string LengthUnit { get; set; } = "yd";
    public string LotNumber { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
}

public class CraftingToolFormRequest
{
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
}

public class CraftingNotionFormRequest
{
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
}

public class PrintingMaterialFormRequest
{
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public decimal SpoolWeightGrams { get; set; } = 1000;
    public decimal? RemainingWeightGrams { get; set; }
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public string SpoolCode { get; set; } = string.Empty;
    public string PrinterCompatibility { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
}

public class CraftingKitFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public int Difficulty { get; set; }
    public int Progress { get; set; }
}

public class CraftingKitPieceFormRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CraftingKitSupplyFormRequest
{
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CraftingKitProjectFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public Guid? ThemeId { get; set; }
    public int? Difficulty { get; set; }
}

public class InventoryPurchaseSummary
{
    public string SourceName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsSalePrice { get; set; }
}

public class InventoryReferenceItemSummary
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CraftingYarnLotSummary
{
    public string LotNumber { get; set; } = string.Empty;
    public decimal Skeins { get; set; }
    public decimal? RemainingLength { get; set; }
}

public class CraftingYarnInventoryItemSummary
{
    public Guid Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string MainColor { get; set; } = string.Empty;
    public string WeightName { get; set; } = string.Empty;
    public string FiberTag { get; set; } = string.Empty;
    public decimal TotalSkeins { get; set; }
    public decimal? EstimatedRemainingLength { get; set; }
    public string LengthUnit { get; set; } = string.Empty;
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<CraftingYarnLotSummary> Lots { get; set; } = [];
    public IReadOnlyList<InventoryPurchaseSummary> Purchases { get; set; } = [];
}

public class CraftingToolInventoryItemSummary
{
    public Guid Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<InventoryPurchaseSummary> Purchases { get; set; } = [];
}

public class CraftingNotionInventoryItemSummary
{
    public Guid Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<InventoryPurchaseSummary> Purchases { get; set; } = [];
}

public class PrintingSpoolSummary
{
    public string SpoolCode { get; set; } = string.Empty;
    public decimal StartingWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string PrinterCompatibility { get; set; } = string.Empty;
}

public class PrintingMaterialInventoryItemSummary
{
    public Guid Id { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public decimal TotalSpoolWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public decimal? RegularPrice { get; set; }
    public IReadOnlyList<PrintingSpoolSummary> Spools { get; set; } = [];
    public IReadOnlyList<InventoryPurchaseSummary> Purchases { get; set; } = [];
}

public class CraftingKitSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;
    public int Progress { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public IReadOnlyList<CraftingKitPieceSummary> Pieces { get; set; } = [];
    public IReadOnlyList<CraftingKitSupplySummary> Supplies { get; set; } = [];
}

public class CraftingKitPieceSummary
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PatternId { get; set; }
    public string PatternName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CraftingKitSupplySummary
{
    public Guid Id { get; set; }
    public Guid KitId { get; set; }
    public string SupplyType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int SortOrder { get; set; }
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
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int CompletedStepCount { get; set; }
    public int TotalStepCount { get; set; }
    public long TotalTrackedSeconds { get; set; }
    public bool TimerRunning { get; set; }
    public DateTime? TimerStartedAt { get; set; }
    public IReadOnlyList<CraftingProjectStepProgressDetail> StepProgress { get; set; } = [];
    public IReadOnlyList<CraftingProjectTimerDetail> Timers { get; set; } = [];
    public IReadOnlyList<CraftingProjectInventoryLinkDetail> InventoryLinks { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CraftingProjectInventoryLinkDetail
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string InventoryItemType { get; set; } = string.Empty;
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public decimal? QuantityPlanned { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CraftingProjectStepProgressDetail
{
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class CraftingProjectTimerDetail
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PatternStepId { get; set; }
    public long ElapsedSeconds { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
