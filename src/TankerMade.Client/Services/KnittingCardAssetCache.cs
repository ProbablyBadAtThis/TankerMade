using TankerMade.Contracts.DTOs.Assets;

namespace TankerMade.Client.Services;

public class KnittingCardAssetCache
{
    private readonly TankerMadeApiClient _apiClient;
    private readonly Dictionary<string, IReadOnlyList<AssetRecordDto>> cache = new(StringComparer.OrdinalIgnoreCase);

    public KnittingCardAssetCache(TankerMadeApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Guid?> GetPrimaryAssetIdAsync(string moduleKey, string recordType, Guid recordId)
    {
        var assets = await GetAssetsAsync(moduleKey, recordType);
        return assets.FirstOrDefault(asset => asset.RecordId == recordId)?.Id;
    }

    public async Task<IReadOnlyList<AssetRecordDto>> GetAssetsAsync(string moduleKey, string recordType)
    {
        var cacheKey = $"{moduleKey}:{recordType}";
        if (cache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var assets = await _apiClient.GetModuleAssetsAsync(moduleKey, recordType);
        cache[cacheKey] = assets;
        return assets;
    }

    public void Invalidate(string moduleKey, string recordType)
    {
        cache.Remove($"{moduleKey}:{recordType}");
    }

    public void InvalidateAll()
    {
        cache.Clear();
    }
}
