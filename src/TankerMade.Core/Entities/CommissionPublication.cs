namespace TankerMade.Core.Entities;

public class CommissionPublication
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime? RevokedAt { get; set; }
    public DateTime LastPublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SnapshotJson { get; set; } = string.Empty;
    public DateTime? HostedAt { get; set; }

    protected CommissionPublication()
    {
    }

    public CommissionPublication(
        Guid id,
        Guid userId,
        string moduleKey,
        Guid projectId,
        string tokenHash,
        string snapshotJson,
        DateTime publishedAt)
    {
        Id = id;
        UserId = userId;
        ModuleKey = Required(moduleKey, nameof(moduleKey), 100);
        ProjectId = projectId;
        TokenHash = Required(tokenHash, nameof(tokenHash), 128);
        SnapshotJson = Required(snapshotJson, nameof(snapshotJson), 100_000);
        CreatedAt = publishedAt;
        LastPublishedAt = publishedAt;
    }

    public bool IsRevoked => RevokedAt.HasValue;

    public void ReplaceSnapshot(string snapshotJson, DateTime publishedAt)
    {
        SnapshotJson = Required(snapshotJson, nameof(snapshotJson), 100_000);
        LastPublishedAt = publishedAt;
        RevokedAt = null;
    }

    public void RotateToken(string tokenHash, string snapshotJson, DateTime publishedAt)
    {
        TokenHash = Required(tokenHash, nameof(tokenHash), 128);
        ReplaceSnapshot(snapshotJson, publishedAt);
    }

    public void ReplaceToken(string tokenHash)
    {
        TokenHash = Required(tokenHash, nameof(tokenHash), 128);
    }

    public void Revoke(DateTime revokedAt)
    {
        RevokedAt = revokedAt;
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
