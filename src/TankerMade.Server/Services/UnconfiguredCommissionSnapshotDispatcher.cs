namespace TankerMade.Server.Services;

public sealed class UnconfiguredCommissionSnapshotDispatcher : ICommissionSnapshotDispatcher
{
    public Task<bool> TryDeliverAsync(
        Guid publicationId,
        string? issuedToken,
        string snapshotJson,
        IReadOnlyList<CommissionSnapshotPhoto> photos,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> TryRevokeAsync(Guid publicationId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
