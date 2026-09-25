using Blazored.LocalStorage;

namespace TankerMade.Client.Services;

public sealed class DashboardLayoutPreferences
{
    private const string StorageKey = "tankermade.dashboard.layout";

    private readonly ILocalStorageService _localStorage;

    public DashboardLayoutPreferences(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action? Changed;

    public bool ShowActivitySummary { get; private set; } = true;
    public bool ShowQuickActions { get; private set; } = true;
    public bool ShowDueSoon { get; private set; } = true;
    public bool ShowEngagement { get; private set; } = true;
    public bool ShowAdminHealth { get; private set; } = true;

    public async Task InitializeAsync()
    {
        var saved = await _localStorage.GetItemAsync<DashboardLayoutPreferencesState>(StorageKey);
        if (saved == null)
        {
            return;
        }

        ShowActivitySummary = saved.ShowActivitySummary;
        ShowQuickActions = saved.ShowQuickActions;
        ShowDueSoon = saved.ShowDueSoon;
        ShowEngagement = saved.ShowEngagement;
        ShowAdminHealth = saved.ShowAdminHealth;
    }

    public async Task SetShowActivitySummaryAsync(bool value)
    {
        ShowActivitySummary = value;
        await SaveAsync();
    }

    public async Task SetShowQuickActionsAsync(bool value)
    {
        ShowQuickActions = value;
        await SaveAsync();
    }

    public async Task SetShowDueSoonAsync(bool value)
    {
        ShowDueSoon = value;
        await SaveAsync();
    }

    public async Task SetShowEngagementAsync(bool value)
    {
        ShowEngagement = value;
        await SaveAsync();
    }

    public async Task SetShowAdminHealthAsync(bool value)
    {
        ShowAdminHealth = value;
        await SaveAsync();
    }

    private async Task SaveAsync()
    {
        await _localStorage.SetItemAsync(StorageKey, new DashboardLayoutPreferencesState
        {
            ShowActivitySummary = ShowActivitySummary,
            ShowQuickActions = ShowQuickActions,
            ShowDueSoon = ShowDueSoon,
            ShowEngagement = ShowEngagement,
            ShowAdminHealth = ShowAdminHealth,
        });

        Changed?.Invoke();
    }

    private sealed class DashboardLayoutPreferencesState
    {
        public bool ShowActivitySummary { get; set; } = true;
        public bool ShowQuickActions { get; set; } = true;
        public bool ShowDueSoon { get; set; } = true;
        public bool ShowEngagement { get; set; } = true;
        public bool ShowAdminHealth { get; set; } = true;
    }
}
