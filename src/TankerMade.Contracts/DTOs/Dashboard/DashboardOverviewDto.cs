namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class DashboardOverviewDto
{
    public DashboardActivitySummaryDto Activity { get; set; } = new();
    public IReadOnlyList<DashboardQuickActionDto> QuickActions { get; set; } = [];
    public IReadOnlyList<DashboardDueSoonItemDto> DueSoonItems { get; set; } = [];
}
