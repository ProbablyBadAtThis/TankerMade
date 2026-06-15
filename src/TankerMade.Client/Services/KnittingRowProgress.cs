using Blazored.LocalStorage;

namespace TankerMade.Client.Services;

public class KnittingRowProgress
{
    private readonly ILocalStorageService _localStorage;

    public KnittingRowProgress(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<bool> IsRowCheckedAsync(Guid projectId, Guid stepId, int row)
    {
        var map = await LoadMapAsync(projectId);
        return map.TryGetValue(BuildKey(stepId, row), out var value) && value;
    }

    public async Task SetRowCheckedAsync(Guid projectId, Guid stepId, int row, bool isChecked)
    {
        var map = await LoadMapAsync(projectId);
        var key = BuildKey(stepId, row);
        if (isChecked)
        {
            map[key] = true;
        }
        else
        {
            map.Remove(key);
        }

        await SaveMapAsync(projectId, map);
    }

    public async Task<IReadOnlyList<int>> GetCheckedRowsAsync(Guid projectId, Guid stepId, IEnumerable<int> rows)
    {
        var map = await LoadMapAsync(projectId);
        return rows.Where(row => map.TryGetValue(BuildKey(stepId, row), out var value) && value).ToList();
    }

    public async Task SyncStepCompletionAsync(
        Guid projectId,
        Guid stepId,
        IEnumerable<int> rows,
        bool stepComplete)
    {
        var map = await LoadMapAsync(projectId);
        var rowList = rows.ToList();
        foreach (var row in rowList)
        {
            var key = BuildKey(stepId, row);
            if (stepComplete)
            {
                map[key] = true;
            }
            else
            {
                map.Remove(key);
            }
        }

        await SaveMapAsync(projectId, map);
    }

    private static string BuildKey(Guid stepId, int row) => $"{stepId:N}:{row}";

    private static string StorageKey(Guid projectId) => $"knitting.rowProgress.{projectId:N}";

    private async Task<Dictionary<string, bool>> LoadMapAsync(Guid projectId)
    {
        return await _localStorage.GetItemAsync<Dictionary<string, bool>>(StorageKey(projectId)) ?? [];
    }

    private async Task SaveMapAsync(Guid projectId, Dictionary<string, bool> map)
    {
        await _localStorage.SetItemAsync(StorageKey(projectId), map);
    }
}
