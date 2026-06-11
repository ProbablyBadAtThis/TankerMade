using Blazored.LocalStorage;

namespace TankerMade.Client.Services;

public class KnittingRecentActivity
{
    private const string RecentProjectKey = "knitting.recentProjectId";
    private const string RecentProjectOpenedAtKey = "knitting.recentProjectOpenedAt";
    private readonly ILocalStorageService _localStorage;

    public KnittingRecentActivity(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task RecordProjectOpenedAsync(Guid projectId)
    {
        await _localStorage.SetItemAsync(RecentProjectKey, projectId);
        await _localStorage.SetItemAsync(RecentProjectOpenedAtKey, DateTime.UtcNow);
    }

    public async Task<Guid?> GetRecentProjectIdAsync()
    {
        return await _localStorage.GetItemAsync<Guid?>(RecentProjectKey);
    }

    public async Task<DateTime?> GetRecentProjectOpenedAtAsync()
    {
        return await _localStorage.GetItemAsync<DateTime?>(RecentProjectOpenedAtKey);
    }

    public async Task ClearRecentProjectAsync()
    {
        await _localStorage.RemoveItemAsync(RecentProjectKey);
        await _localStorage.RemoveItemAsync(RecentProjectOpenedAtKey);
    }
}
