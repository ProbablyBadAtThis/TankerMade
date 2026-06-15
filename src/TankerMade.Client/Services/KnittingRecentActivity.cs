using Blazored.LocalStorage;

namespace TankerMade.Client.Services;

public class KnittingRecentActivity
{
    private const string RecentProjectKey = "knitting.recentProjectId";
    private const string RecentProjectOpenedAtKey = "knitting.recentProjectOpenedAt";
    private const string WorkedOnProjectKey = "knitting.workedOnProjectId";
    private const string WorkedOnAtKey = "knitting.workedOnAt";
    private const string RecentlyViewedKey = "knitting.recentlyViewedProjects";
    private const int MaxRecentlyViewed = 5;
    private readonly ILocalStorageService _localStorage;

    public KnittingRecentActivity(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task RecordProjectOpenedAsync(Guid projectId)
    {
        await _localStorage.SetItemAsync(RecentProjectKey, projectId);
        await _localStorage.SetItemAsync(RecentProjectOpenedAtKey, DateTime.UtcNow);
        await AddRecentlyViewedAsync(projectId);
    }

    public async Task RecordProjectWorkedOnAsync(Guid projectId)
    {
        await _localStorage.SetItemAsync(WorkedOnProjectKey, projectId);
        await _localStorage.SetItemAsync(WorkedOnAtKey, DateTime.UtcNow);
        await _localStorage.SetItemAsync(RecentProjectKey, projectId);
        await _localStorage.SetItemAsync(RecentProjectOpenedAtKey, DateTime.UtcNow);
    }

    public async Task<Guid?> GetRecentProjectIdAsync()
        => await _localStorage.GetItemAsync<Guid?>(RecentProjectKey);

    public async Task<DateTime?> GetRecentProjectOpenedAtAsync()
        => await _localStorage.GetItemAsync<DateTime?>(RecentProjectOpenedAtKey);

    public async Task<Guid?> GetWorkedOnProjectIdAsync()
        => await _localStorage.GetItemAsync<Guid?>(WorkedOnProjectKey);

    public async Task<DateTime?> GetWorkedOnAtAsync()
        => await _localStorage.GetItemAsync<DateTime?>(WorkedOnAtKey);

    public async Task<IReadOnlyList<Guid>> GetRecentlyViewedProjectIdsAsync(Guid? excludeProjectId = null)
    {
        var entries = await _localStorage.GetItemAsync<List<RecentViewEntry>>(RecentlyViewedKey) ?? [];
        return entries
            .Where(entry => !excludeProjectId.HasValue || entry.ProjectId != excludeProjectId.Value)
            .Select(entry => entry.ProjectId)
            .ToList();
    }

    public async Task ClearRecentProjectAsync()
    {
        await _localStorage.RemoveItemAsync(RecentProjectKey);
        await _localStorage.RemoveItemAsync(RecentProjectOpenedAtKey);
    }

    private async Task AddRecentlyViewedAsync(Guid projectId)
    {
        var entries = await _localStorage.GetItemAsync<List<RecentViewEntry>>(RecentlyViewedKey) ?? [];
        entries.RemoveAll(entry => entry.ProjectId == projectId);
        entries.Insert(0, new RecentViewEntry
        {
            ProjectId = projectId,
            ViewedAt = DateTime.UtcNow
        });

        if (entries.Count > MaxRecentlyViewed)
        {
            entries = entries.Take(MaxRecentlyViewed).ToList();
        }

        await _localStorage.SetItemAsync(RecentlyViewedKey, entries);
    }

    private sealed class RecentViewEntry
    {
        public Guid ProjectId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
