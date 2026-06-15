using Blazored.LocalStorage;

namespace TankerMade.Client.Services;

public class CoreRecentModule
{
    private const string RecentModuleKey = "core.recentModuleKey";
    private const string RecentModuleVisitedAtKey = "core.recentModuleVisitedAt";
    private readonly ILocalStorageService _localStorage;

    public CoreRecentModule(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task RecordModuleVisitAsync(string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return;
        }

        await _localStorage.SetItemAsync(RecentModuleKey, moduleKey);
        await _localStorage.SetItemAsync(RecentModuleVisitedAtKey, DateTime.UtcNow);
    }

    public async Task<string?> GetRecentModuleKeyAsync()
        => await _localStorage.GetItemAsync<string?>(RecentModuleKey);

    public async Task<DateTime?> GetRecentModuleVisitedAtAsync()
        => await _localStorage.GetItemAsync<DateTime?>(RecentModuleVisitedAtKey);
}
