namespace TankerMade.Server.Services;

public sealed record CommissionSnapshotPhoto(Guid AssetId, string ContentType, byte[] Content);

public interface ICommissionSnapshotDispatcher
{
    Task<bool> TryDeliverAsync(
        Guid publicationId,
        string? issuedToken,
        string snapshotJson,
        IReadOnlyList<CommissionSnapshotPhoto> photos,
        CancellationToken cancellationToken = default);

    Task<bool> TryRevokeAsync(Guid publicationId, CancellationToken cancellationToken = default);
}
