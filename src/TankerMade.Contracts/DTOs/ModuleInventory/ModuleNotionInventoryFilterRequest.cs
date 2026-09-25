namespace TankerMade.Contracts.DTOs.ModuleInventory;

public class ModuleNotionInventoryFilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string Search { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
}
