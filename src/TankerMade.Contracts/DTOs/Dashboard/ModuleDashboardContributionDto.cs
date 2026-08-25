namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class ModuleDashboardContributionDto
{
    public int ActiveProjectCount { get; set; }
    public IReadOnlyList<DashboardQuickActionDto> QuickActions { get; set; } = [];
    public IReadOnlyList<DashboardDueSoonItemDto> DueSoonItems { get; set; } = [];
    public IReadOnlyList<DashboardAttentionItemDto> AttentionItems { get; set; } = [];
}
