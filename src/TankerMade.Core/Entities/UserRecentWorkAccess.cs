namespace TankerMade.Core.Entities;

public class UserRecentWorkAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string WorkItemType { get; set; } = string.Empty;
    public Guid WorkItemId { get; set; }
    public DateTime LastAccessedAtUtc { get; set; }

    protected UserRecentWorkAccess()
    {
    }

    public UserRecentWorkAccess(
        Guid id,
        Guid userId,
        string moduleKey,
        string workItemType,
        Guid workItemId,
        DateTime lastAccessedAtUtc)
    {
        Id = id;
        UserId = userId;
        ModuleKey = Required(moduleKey, nameof(moduleKey), 100);
        WorkItemType = Required(workItemType, nameof(workItemType), 50);
        WorkItemId = workItemId;
        LastAccessedAtUtc = lastAccessedAtUtc;
    }

    public void Touch(DateTime accessedAtUtc)
    {
        LastAccessedAtUtc = accessedAtUtc;
    }

    private static string Required(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Value exceeds max length of {maxLength}.", parameterName);
        }

        return trimmed;
    }
}
