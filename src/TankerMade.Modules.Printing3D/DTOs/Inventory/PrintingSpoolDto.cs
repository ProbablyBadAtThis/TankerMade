namespace TankerMade.Modules.Printing3D.DTOs.Inventory;

public class PrintingSpoolDto
{
    public Guid Id { get; set; }
    public Guid MaterialInventoryItemId { get; set; }
    public string SpoolCode { get; set; } = string.Empty;
    public decimal StartingWeightGrams { get; set; }
    public decimal? RemainingWeightGrams { get; set; }
    public string PrinterCompatibility { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
