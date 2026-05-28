namespace TankerMade.Contracts.DTOs.Assets;

public class StoredAssetFileResult
{
    public string StorageProvider { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
