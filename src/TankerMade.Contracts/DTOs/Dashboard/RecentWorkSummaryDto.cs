namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class RecentWorkSummaryDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string WorkItemType { get; set; } = string.Empty;
    public Guid WorkItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid? ThumbnailAssetId { get; set; }
    public string? ThumbnailFallbackPath { get; set; }
    public DateTime LastAccessedAtUtc { get; set; }
    public string NavigationPath { get; set; } = string.Empty;
}
