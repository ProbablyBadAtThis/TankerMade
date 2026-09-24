using System.Text.Json.Serialization;
using TankerMade.Core.Enums;

namespace TankerMade.Contracts.DTOs.ClientProgress;

public class ClientProgressMaterialLineDto
{
    public string Name { get; set; } = string.Empty;
}

public class ClientProgressRevisionDto
{
    public DateTime OccurredAt { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool PriceChanged { get; set; }
    public bool DueDateChanged { get; set; }
    public decimal? PreviousPrice { get; set; }
    public decimal? NewPrice { get; set; }
    public DateOnly? PreviousDueDate { get; set; }
    public DateOnly? NewDueDate { get; set; }
}

public class ClientProgressSnapshotDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CommissionStage Stage { get; set; }

    public string Summary { get; set; } = string.Empty;
    public List<ClientProgressMaterialLineDto> MaterialLines { get; set; } = [];
    public List<Guid> PhotoAssetIds { get; set; } = [];
    public string NextStep { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ClientProgressRevisionDto> Revisions { get; set; } = [];
}

public class ClientProgressPublishChoices
{
    public CommissionStage Stage { get; set; }
    public string? PublicSummary { get; set; }
    public List<string> MaterialNames { get; set; } = [];
    public List<Guid> PhotoAssetIds { get; set; } = [];
    public string? NextStep { get; set; }
    public decimal? Price { get; set; }
    public DateOnly? DueDate { get; set; }
}

public class ClientProgressRevisionInput
{
    public string? Summary { get; set; }
    public decimal? PreviousPrice { get; set; }
    public decimal? NewPrice { get; set; }
    public DateOnly? PreviousDueDate { get; set; }
    public DateOnly? NewDueDate { get; set; }
}

public class PublishClientProgressRequest
{
    public CommissionStage Stage { get; set; }
    public string? PublicSummary { get; set; }
    public List<string> MaterialNames { get; set; } = [];
    public List<Guid> PhotoAssetIds { get; set; } = [];
    public string? NextStep { get; set; }
    public ClientProgressRevisionInput? Revision { get; set; }

    public decimal? QuotePrice { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool DepositReceived { get; set; }
    public decimal? TargetHourlyRate { get; set; }
    public decimal? MaterialCost { get; set; }
    public int? ElapsedSeconds { get; set; }
    public decimal? ImpliedNet { get; set; }
    public bool? BeatsRate { get; set; }
    public string? PrivateNotes { get; set; }
    public string? Measurements { get; set; }
}

public class AddClientProgressRevisionRequest
{
    public string? Summary { get; set; }
    public decimal? PreviousPrice { get; set; }
    public decimal? NewPrice { get; set; }
    public DateOnly? PreviousDueDate { get; set; }
    public DateOnly? NewDueDate { get; set; }
}

public enum CommissionCommandStatus
{
    Ok,
    NotFound,
    ModuleInactive
}

public class CommissionCommandResult
{
    public CommissionCommandStatus Status { get; set; }
    public ClientProgressSnapshotDto? Snapshot { get; set; }
    public string? IssuedToken { get; set; }
}

public class PublishedClientPhoto
{
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
}

public class ClientProgressWorkFacts
{
    public decimal MaterialCost { get; set; }
    public long ElapsedSeconds { get; set; }
    public List<string> MaterialNames { get; set; } = [];
}

public class UpdateCommissionTermsRequest
{
    public decimal? QuotePrice { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool DepositReceived { get; set; }
}

public class CommissionWorkshopDto
{
    public decimal? TargetHourlyRate { get; set; }
    public decimal? QuotePrice { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool DepositReceived { get; set; }
    public decimal MaterialCost { get; set; }
    public long ElapsedSeconds { get; set; }
    public decimal? ImpliedNet { get; set; }
    public decimal? ImpliedHourly { get; set; }
    public bool? BeatsRate { get; set; }
    public string SuggestedSummary { get; set; } = string.Empty;
    public string SuggestedNextStep { get; set; } = string.Empty;
    public List<string> SuggestedMaterialNames { get; set; } = [];
    public bool HasActiveLink { get; set; }
    public DateTime? LastPublishedAt { get; set; }
}

public class MakerRateDto
{
    public decimal? TargetHourlyRate { get; set; }
}
