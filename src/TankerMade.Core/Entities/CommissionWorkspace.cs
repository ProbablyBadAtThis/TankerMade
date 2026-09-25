namespace TankerMade.Core.Entities;

public class CommissionWorkspace
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public decimal? QuotePrice { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool DepositReceived { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected CommissionWorkspace()
    {
    }

    public CommissionWorkspace(Guid id, Guid userId, string moduleKey, Guid projectId, DateTime updatedAt)
    {
        Id = id;
        UserId = userId;
        ModuleKey = moduleKey.Trim();
        ProjectId = projectId;
        UpdatedAt = updatedAt;
    }

    public void Update(decimal? quotePrice, DateOnly? dueDate, bool depositReceived, DateTime updatedAt)
    {
        if (quotePrice is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quotePrice), "Quote price cannot be negative.");
        }

        QuotePrice = quotePrice;
        DueDate = dueDate;
        DepositReceived = depositReceived;
        UpdatedAt = updatedAt;
    }
}
