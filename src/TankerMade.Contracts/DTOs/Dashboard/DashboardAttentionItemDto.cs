namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class DashboardAttentionItemDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string NavigationPath { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
