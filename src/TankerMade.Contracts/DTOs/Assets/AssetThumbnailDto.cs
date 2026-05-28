namespace TankerMade.Contracts.DTOs.Assets;

public class AssetThumbnailDto
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
}
