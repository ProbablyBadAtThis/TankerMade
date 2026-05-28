namespace TankerMade.Contracts.DTOs.Assets;

public class AssetAssignmentRequest
{
    public string ModuleKey { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public Guid? RecordId { get; set; }
}
