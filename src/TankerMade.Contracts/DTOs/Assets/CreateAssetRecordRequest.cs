namespace TankerMade.Contracts.DTOs.Assets;

public class CreateAssetRecordRequest
{
    public string ModuleKey { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public Guid? RecordId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
