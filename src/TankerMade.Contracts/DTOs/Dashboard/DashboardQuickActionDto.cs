namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class DashboardQuickActionDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string NavigationPath { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
