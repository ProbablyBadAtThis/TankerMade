namespace TankerMade.Modules.Printing3D.DTOs.Inventory;

public class PrintingMaterialInventoryFilterDto
{
    public string Search { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string Diameter { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
}
