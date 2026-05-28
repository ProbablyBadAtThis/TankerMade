namespace TankerMade.Core.Entities;

public class AssetThumbnail
{
    public Guid Id { get; set; }
    public Guid AssetRecordId { get; set; }
    public string SizeKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }

    protected AssetThumbnail()
    {
    }

    public AssetThumbnail(
        Guid id,
        Guid assetRecordId,
        string sizeKey,
        string contentType,
        int width,
        int height,
        string storageProvider,
        string storagePath,
        long fileSizeBytes)
    {
        Id = id;
        AssetRecordId = assetRecordId;
        SizeKey = Required(sizeKey, nameof(sizeKey), 50);
        ContentType = Required(contentType, nameof(contentType), 150);
        Width = width > 0 ? width : throw new ArgumentOutOfRangeException(nameof(width));
        Height = height > 0 ? height : throw new ArgumentOutOfRangeException(nameof(height));
        StorageProvider = Required(storageProvider, nameof(storageProvider), 50);
        StoragePath = Required(storagePath, nameof(storagePath), 500);
        FileSizeBytes = fileSizeBytes >= 0
            ? fileSizeBytes
            : throw new ArgumentOutOfRangeException(nameof(fileSizeBytes));
        CreatedAt = DateTime.UtcNow;
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
