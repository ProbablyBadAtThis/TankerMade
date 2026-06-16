using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Client.Pages;

public partial class KnittingDashboard
{
    private RecentWorkSummaryDto? featuredRecentWork;
    private IReadOnlyList<RecentWorkSummaryDto> sidebarRecentWork = [];
    private DashboardOverviewDto? overview;

    private static string? GetThumbnailFallback(RecentWorkSummaryDto item)
        => string.IsNullOrWhiteSpace(item.ThumbnailFallbackPath)
            ? null
            : item.ThumbnailFallbackPath;

    private static int GetProgress(RecentWorkSummaryDto item)
        => item.ProgressPercent ?? 0;

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

    private static bool HasEngagementSignal(DashboardActivitySummaryDto activity)
        => activity.WorkStreakDays > 0 || activity.ActiveDaysThisWeek > 0;

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
}
