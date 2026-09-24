using TankerMade.Contracts.DTOs.ClientProgress;

namespace TankerMade.Contracts.Services;

public interface ICommissionPublicationService
{
    Task<CommissionWorkshopDto?> GetWorkshopAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<CommissionCommandResult> SaveTermsAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        UpdateCommissionTermsRequest request,
        CancellationToken cancellationToken = default);

    Task<CommissionCommandResult> ReissueLinkAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<MakerRateDto?> GetMakerRateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<MakerRateDto?> SetMakerRateAsync(Guid userId, decimal? targetHourlyRate, CancellationToken cancellationToken = default);

    Task<CommissionCommandResult> PublishAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        PublishClientProgressRequest request,
        CancellationToken cancellationToken = default);

    Task<CommissionCommandResult> AddRevisionAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        AddClientProgressRevisionRequest request,
        CancellationToken cancellationToken = default);

    Task<CommissionCommandResult> RevokeAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<ClientProgressSnapshotDto?> GetPreviewAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<ClientProgressSnapshotDto?> GetByTokenAsync(
        string? token,
        CancellationToken cancellationToken = default);

    Task<PublishedClientPhoto?> OpenPhotoAsync(
        string? token,
        Guid assetId,
        CancellationToken cancellationToken = default);
}
