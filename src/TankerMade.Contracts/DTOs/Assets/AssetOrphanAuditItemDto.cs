namespace TankerMade.Contracts.DTOs.Assets;

public class AssetOrphanAuditItemDto
{
    public Guid AssetId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public bool FileMissing { get; set; }
    public bool HasThumbnailIssues { get; set; }
    public int MissingThumbnailCount { get; set; }
}
