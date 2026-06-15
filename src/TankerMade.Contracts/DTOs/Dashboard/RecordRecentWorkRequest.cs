namespace TankerMade.Contracts.DTOs.Dashboard;

public sealed class RecordRecentWorkRequest
{
    public string ModuleKey { get; set; } = string.Empty;
    public string WorkItemType { get; set; } = RecentWorkTypes.Project;
    public Guid WorkItemId { get; set; }
}
