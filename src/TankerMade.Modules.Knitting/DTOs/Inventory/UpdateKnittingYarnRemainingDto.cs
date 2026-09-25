namespace TankerMade.Modules.Knitting.DTOs.Inventory;

public class UpdateKnittingYarnRemainingDto
{
    public decimal? EstimatedRemainingLength { get; set; }
    public string? LengthUnit { get; set; }
    public decimal? CurrentWeight { get; set; }
}
