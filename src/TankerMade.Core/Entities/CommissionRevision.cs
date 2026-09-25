namespace TankerMade.Core.Entities;

public class CommissionRevision
{
    public Guid Id { get; set; }
    public Guid PublicationId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool PriceChanged { get; set; }
    public bool DueDateChanged { get; set; }
    public decimal? PreviousPrice { get; set; }
    public decimal? NewPrice { get; set; }
    public DateOnly? PreviousDueDate { get; set; }
    public DateOnly? NewDueDate { get; set; }

    protected CommissionRevision()
    {
    }

    public CommissionRevision(
        Guid id,
        Guid publicationId,
        DateTime occurredAt,
        string summary,
        bool priceChanged,
        bool dueDateChanged,
        decimal? previousPrice,
        decimal? newPrice,
        DateOnly? previousDueDate,
        DateOnly? newDueDate)
    {
        Id = id;
        PublicationId = publicationId;
        OccurredAt = occurredAt;
        Summary = string.IsNullOrWhiteSpace(summary) ? string.Empty : summary.Trim();
        if (Summary.Length > 500)
        {
            throw new ArgumentException("Summary exceeds max length of 500.", nameof(summary));
        }

        PriceChanged = priceChanged;
        DueDateChanged = dueDateChanged;
        PreviousPrice = previousPrice;
        NewPrice = newPrice;
        PreviousDueDate = previousDueDate;
        NewDueDate = newDueDate;
    }
}
