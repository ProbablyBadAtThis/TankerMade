using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using TankerMade.Client.Services;
using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Client.Pages;

public partial class Home : IDisposable
{
    private bool initialized;
    private string errorMessage = string.Empty;
    private IReadOnlyList<ClientModuleSummary> activeModules = [];
    private RecentWorkSummaryDto? featuredRecentWork;
    private IReadOnlyList<RecentWorkSummaryDto> sidebarRecentWork = [];
    private DashboardOverviewDto? overview;
    private bool _loadInProgress;

    private bool HasActiveModules => activeModules.Count > 0;
    private bool IsAdmin => string.Equals(AuthSession.State.Role, "Admin", StringComparison.OrdinalIgnoreCase);
    private bool ShowEngagementSidebar => initialized && LayoutPreferences.ShowEngagement && overview != null;

    protected override async Task OnInitializedAsync()
    {
        AuthSession.Changed += HandleAuthChanged;
        ModuleState.Changed += HandleModuleStateChanged;
        LayoutPreferences.Changed += HandleLayoutPreferencesChanged;
        Navigation.LocationChanged += OnLocationChanged;
        await AuthSession.InitializeAsync();
        await LayoutPreferences.InitializeAsync();

        if (!AuthSession.IsSignedIn)
        {
            Navigation.NavigateTo("/");
            return;
        }

        await LoadDashboardAsync(refreshModules: true);
        initialized = true;
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (!Navigation.ToBaseRelativePath(e.Location).TrimEnd('/').Equals("home", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await LoadDashboardAsync(refreshModules: true);
        await InvokeAsync(StateHasChanged);
    }

    private async void HandleModuleStateChanged()
    {
        if (!initialized)
        {
            return;
        }

        await LoadDashboardAsync(refreshModules: false);
        await InvokeAsync(StateHasChanged);
    }

    private void HandleLayoutPreferencesChanged() => InvokeAsync(StateHasChanged);

    private async Task LoadDashboardAsync(bool refreshModules)
    {
        if (_loadInProgress)
        {
            return;
        }

        _loadInProgress = true;
        errorMessage = string.Empty;
        featuredRecentWork = null;
        sidebarRecentWork = [];
        overview = null;

        try
        {
            if (refreshModules)
            {
                await ModuleState.RefreshAsync();
            }

            activeModules = ModuleState.AvailableModules
                .Where(module => ModuleState.IsActive(module.ModuleKey))
                .OrderBy(module => module.NavigationOrder)
                .ThenBy(module => module.Name)
                .ToList();

            var recentWorkTask = ApiClient.GetRecentWorkAsync(limit: 5);
            var overviewTask = ApiClient.GetDashboardOverviewAsync();
            await Task.WhenAll(recentWorkTask, overviewTask);

            var recentWork = await recentWorkTask;
            overview = await overviewTask;
            featuredRecentWork = recentWork.FirstOrDefault();
            sidebarRecentWork = recentWork.Skip(1).ToList();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            activeModules = [];
        }
        finally
        {
            _loadInProgress = false;
        }
    }

    private string GetGreeting()
    {
        var username = AuthSession.State.Username;
        var name = string.IsNullOrWhiteSpace(username) ? "there" : username;
        var hour = DateTime.Now.Hour;

        if (hour >= 17)
        {
            return $"Good evening, {name}";
        }

        if (hour >= 12)
        {
            return $"Good afternoon, {name}";
        }

        return $"Good morning, {name}";
    }

    private string GetModuleName(string moduleKey)
    {
        var module = ModuleState.AvailableModules.FirstOrDefault(item =>
            string.Equals(item.ModuleKey, moduleKey, StringComparison.OrdinalIgnoreCase));

        return module?.Name ?? moduleKey;
    }

    private static string GetModuleRoute(ClientModuleSummary module)
        => string.IsNullOrWhiteSpace(module.NavigationRoute)
            ? $"/modules/{module.ModuleKey}"
            : $"/{module.NavigationRoute.TrimStart('/')}";

    private static string? GetThumbnailFallback(RecentWorkSummaryDto item)
        => string.IsNullOrWhiteSpace(item.ThumbnailFallbackPath)
            ? null
            : item.ThumbnailFallbackPath;

    private static string FormatActivitySummary(DashboardActivitySummaryDto activity)
    {
        var projectLabel = activity.ActiveProjectCount == 1 ? "active project" : "active projects";
        var moduleLabel = activity.ActiveModuleCount == 1 ? "module" : "modules";
        return $"{activity.ActiveProjectCount} {projectLabel} · {activity.ActiveModuleCount} {moduleLabel}";
    }

    private static bool HasEngagementSignal(DashboardActivitySummaryDto activity)
        => activity.WorkStreakDays > 0 || activity.ActiveDaysThisWeek > 0;

    private string GetEngagementCopy()
    {
        if (overview == null)
        {
            return string.Empty;
        }

        return HasEngagementSignal(overview.Activity)
            ? FormatEngagementSummary(overview.Activity)
            : "Open a project to start building your activity calendar.";
    }

    private IReadOnlyList<QuickActionGroup> GetQuickActionGroups()
    {
        if (overview?.QuickActions.Count is not > 0)
        {
            return [];
        }

        return overview.QuickActions
            .GroupBy(action => action.ModuleKey, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var orderedActions = group
                    .OrderBy(action => action.SortOrder)
                    .ThenBy(action => action.Label, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return new QuickActionGroup(
                    orderedActions[0].ModuleName,
                    orderedActions);
            })
            .OrderBy(group => group.ModuleName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private sealed record QuickActionGroup(string ModuleName, IReadOnlyList<DashboardQuickActionDto> Actions);

    private static string FormatEngagementSummary(DashboardActivitySummaryDto activity)
    {
        if (activity.WorkStreakDays > 0 && activity.ActiveDaysThisWeek > 0)
        {
            var streakLabel = activity.WorkStreakDays == 1 ? "day" : "days";
            var weekLabel = activity.ActiveDaysThisWeek == 1 ? "day" : "days";
            return $"{activity.WorkStreakDays}-{streakLabel} streak · {activity.ActiveDaysThisWeek} active {weekLabel} this week";
        }

        if (activity.WorkStreakDays > 0)
        {
            var streakLabel = activity.WorkStreakDays == 1 ? "day" : "days";
            return $"{activity.WorkStreakDays}-{streakLabel} streak";
        }

        var activeLabel = activity.ActiveDaysThisWeek == 1 ? "day" : "days";
        return $"{activity.ActiveDaysThisWeek} active {activeLabel} this week";
    }

    private static string FormatLastActive(DateTime lastAccessedAtUtc)
    {
        var local = lastAccessedAtUtc.ToLocalTime();
        var delta = DateTime.Now - local;
        if (delta.TotalMinutes < 1)
        {
            return "Active just now";
        }

        if (delta.TotalHours < 1)
        {
            var minutes = Math.Max(1, (int)delta.TotalMinutes);
            return $"Active {minutes} min ago";
        }

        if (delta.TotalDays < 1)
        {
            var hours = Math.Max(1, (int)delta.TotalHours);
            return $"Active {hours} hr ago";
        }

        if (delta.TotalDays < 7)
        {
            var days = Math.Max(1, (int)delta.TotalDays);
            return $"Active {days} day{(days == 1 ? string.Empty : "s")} ago";
        }

        return $"Active {local:g}";
    }

    private static string FormatDueAt(DateTime dueAtUtc)
    {
        var local = dueAtUtc.ToLocalTime();
        var daysUntilDue = (local.Date - DateTime.Today).Days;
        if (daysUntilDue == 0)
        {
            return "Due today";
        }

        if (daysUntilDue == 1)
        {
            return "Due tomorrow";
        }

        if (daysUntilDue > 1 && daysUntilDue <= 7)
        {
            return $"Due in {daysUntilDue} days";
        }

        return $"Due {local:d}";
    }

    private void HandleAuthChanged()
    {
        if (!AuthSession.IsSignedIn)
        {
            ModuleState.Reset();
            activeModules = [];
            featuredRecentWork = null;
            sidebarRecentWork = [];
            overview = null;
        }
    }

    public void Dispose()
    {
        AuthSession.Changed -= HandleAuthChanged;
        ModuleState.Changed -= HandleModuleStateChanged;
        LayoutPreferences.Changed -= HandleLayoutPreferencesChanged;
        Navigation.LocationChanged -= OnLocationChanged;
    }
}
