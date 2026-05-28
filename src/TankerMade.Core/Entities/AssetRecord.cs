namespace TankerMade.Core.Entities;

public class AssetRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public Guid? RecordId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected AssetRecord()
    {
    }

    public AssetRecord(
        Guid id,
        Guid userId,
        string moduleKey,
        string originalFileName,
        string contentType,
        long fileSizeBytes,
        string storageProvider,
        string storagePath,
        string? recordType = null,
        Guid? recordId = null)
    {
        Id = id;
        UserId = userId;
        ModuleKey = Required(moduleKey, nameof(moduleKey), 100);
        OriginalFileName = Required(originalFileName, nameof(originalFileName), 260);
        ContentType = Required(contentType, nameof(contentType), 150);
        FileSizeBytes = fileSizeBytes >= 0
            ? fileSizeBytes
            : throw new ArgumentOutOfRangeException(nameof(fileSizeBytes));
        StorageProvider = Required(storageProvider, nameof(storageProvider), 50);
        StoragePath = Required(storagePath, nameof(storagePath), 500);
        RecordType = Optional(recordType, 100);
        RecordId = recordId;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Reassign(string? recordType, Guid? recordId)
    {
        RecordType = Optional(recordType, 100);
        RecordId = recordId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
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

    private static string Optional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Value exceeds max length of {maxLength}.", nameof(value));
        }

        return trimmed;
    }
}
