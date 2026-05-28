namespace TankerMade.Contracts.DTOs.Assets;

public class AssetRecordDto
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
    public List<AssetThumbnailDto> Thumbnails { get; set; } = [];
}
