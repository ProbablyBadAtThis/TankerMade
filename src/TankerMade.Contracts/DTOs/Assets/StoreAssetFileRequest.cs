namespace TankerMade.Contracts.DTOs.Assets;

public class StoreAssetFileRequest
{
    public Guid AssetId { get; set; }
    public Guid UserId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string VariantKey { get; set; } = string.Empty;
}
