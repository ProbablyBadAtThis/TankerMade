namespace TankerMade.Contracts.DTOs.ModuleInventory;

public class UpdateModuleYarnRemainingRequest
{
    public decimal? EstimatedRemainingLength { get; set; }
    public string? LengthUnit { get; set; }
    public decimal? CurrentWeight { get; set; }
}
