using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services;
using TankerMade.Server.Data;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Services;

public class DashboardOverviewService : IDashboardOverviewService
{
    private const int MaxQuickActions = 8;
    private const int MaxDueSoonItems = 5;
    private const int MaxAttentionItems = 8;

    private readonly TankerMadeDbContext _context;
    private readonly IModuleService _moduleService;
    private readonly IModuleDashboardContributionResolver _contributionResolver;

    public DashboardOverviewService(
        TankerMadeDbContext context,
        IModuleService moduleService,
        IModuleDashboardContributionResolver contributionResolver)
    {
        _context = context;
        _moduleService = moduleService;
        _contributionResolver = contributionResolver;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync(
        Guid userId,
        string? moduleKey = null,
        CancellationToken cancellationToken = default)
    {
        var activeModules = await _moduleService.GetActiveModulesAsync(userId);
        if (!string.IsNullOrWhiteSpace(moduleKey))
        {
            var normalizedModuleKey = moduleKey.Trim();
            activeModules = activeModules
                .Where(module => string.Equals(module.ModuleKey, normalizedModuleKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var moduleNames = activeModules.ToDictionary(
            module => module.ModuleKey,
            module => module.Name,
            StringComparer.OrdinalIgnoreCase);

        var activeProjectCount = 0;
        var quickActions = new List<DashboardQuickActionDto>();
        var dueSoonItems = new List<DashboardDueSoonItemDto>();
        var attentionItems = new List<DashboardAttentionItemDto>();

        foreach (var module in activeModules)
        {
            var provider = _contributionResolver.Resolve(module.ModuleKey);
            if (provider == null)
            {
                continue;
            }

            var contribution = await provider.GetContributionAsync(userId, cancellationToken);
            activeProjectCount += contribution.ActiveProjectCount;

            foreach (var action in contribution.QuickActions)
            {
                quickActions.Add(EnrichQuickAction(action, moduleNames));
            }

            foreach (var dueItem in contribution.DueSoonItems)
            {
                dueSoonItems.Add(EnrichDueSoonItem(dueItem, moduleNames));
            }

            foreach (var attentionItem in contribution.AttentionItems)
            {
                attentionItems.Add(EnrichAttentionItem(attentionItem, moduleNames));
            }
        }

        var accessQuery = _context.UserRecentWorkAccesses
            .AsNoTracking()
            .Where(entry => entry.UserId == userId);

        if (!string.IsNullOrWhiteSpace(moduleKey))
        {
            var normalizedModuleKey = moduleKey.Trim();
            accessQuery = accessQuery.Where(entry => entry.ModuleKey == normalizedModuleKey);
        }

        var accessTimes = await accessQuery
            .Select(entry => entry.LastAccessedAtUtc)
            .ToListAsync(cancellationToken);

        return new DashboardOverviewDto
        {
            Activity = new DashboardActivitySummaryDto
            {
                ActiveProjectCount = activeProjectCount,
                ActiveModuleCount = activeModules.Count,
                WorkStreakDays = CalculateWorkStreakDays(accessTimes),
                ActiveDaysThisWeek = CalculateActiveDaysThisWeek(accessTimes),
                ActiveLocalDates = ExtractActiveLocalDates(accessTimes),
            },
            QuickActions = quickActions
                .OrderBy(action => action.SortOrder)
                .ThenBy(action => action.ModuleName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(action => action.Label, StringComparer.OrdinalIgnoreCase)
                .Take(MaxQuickActions)
                .ToList(),
            DueSoonItems = dueSoonItems
                .OrderBy(item => item.DueAtUtc)
                .Take(MaxDueSoonItems)
                .ToList(),
            AttentionItems = attentionItems
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
                .Take(MaxAttentionItems)
                .ToList(),
        };
    }

    private static DashboardAttentionItemDto EnrichAttentionItem(
        DashboardAttentionItemDto item,
        IReadOnlyDictionary<string, string> moduleNames)
    {
        item.ModuleName = moduleNames.TryGetValue(item.ModuleKey, out var moduleName)
            ? moduleName
            : item.ModuleKey;

        return item;
    }

    private static DashboardQuickActionDto EnrichQuickAction(
        DashboardQuickActionDto action,
        IReadOnlyDictionary<string, string> moduleNames)
    {
        action.ModuleName = moduleNames.TryGetValue(action.ModuleKey, out var moduleName)
            ? moduleName
            : action.ModuleKey;

        return action;
    }

    private static DashboardDueSoonItemDto EnrichDueSoonItem(
        DashboardDueSoonItemDto item,
        IReadOnlyDictionary<string, string> moduleNames)
    {
        item.ModuleName = moduleNames.TryGetValue(item.ModuleKey, out var moduleName)
            ? moduleName
            : item.ModuleKey;

        return item;
    }

    public static int CalculateWorkStreakDays(IReadOnlyList<DateTime> accessTimesUtc)
    {
        if (accessTimesUtc.Count == 0)
        {
            return 0;
        }

        var distinctDays = accessTimesUtc
            .Select(time => time.ToLocalTime().Date)
            .Distinct()
            .OrderByDescending(day => day)
            .ToList();

        var today = DateTime.Today;
        if (distinctDays[0] < today.AddDays(-1))
        {
            return 0;
        }

        var cursor = distinctDays[0] == today ? today : today.AddDays(-1);
        var streak = 0;

        foreach (var day in distinctDays)
        {
            if (day == cursor)
            {
                streak++;
                cursor = cursor.AddDays(-1);
                continue;
            }

            if (day < cursor)
            {
                break;
            }
        }

        return streak;
    }

    public static int CalculateActiveDaysThisWeek(IReadOnlyList<DateTime> accessTimesUtc)
    {
        if (accessTimesUtc.Count == 0)
        {
            return 0;
        }

        var today = DateTime.Today;
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysSinceMonday);

        return accessTimesUtc
            .Select(time => time.ToLocalTime().Date)
            .Distinct()
            .Count(day => day >= weekStart && day <= today);
    }

    public static IReadOnlyList<string> ExtractActiveLocalDates(IReadOnlyList<DateTime> accessTimesUtc)
        => accessTimesUtc
            .Select(time => time.ToLocalTime().Date)
            .Distinct()
            .OrderBy(day => day)
            .Select(day => day.ToString("yyyy-MM-dd"))
            .ToList();
}
