namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class DashboardDueSoonItemDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime DueAtUtc { get; set; }
    public string NavigationPath { get; set; } = string.Empty;
}
