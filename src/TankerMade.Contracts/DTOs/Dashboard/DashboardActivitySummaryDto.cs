namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class DashboardActivitySummaryDto
{
    public int ActiveProjectCount { get; set; }
    public int ActiveModuleCount { get; set; }
    public int WorkStreakDays { get; set; }
    public int ActiveDaysThisWeek { get; set; }
    public IReadOnlyList<string> ActiveLocalDates { get; set; } = [];
}
