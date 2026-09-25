using TankerMade.Contracts.DTOs.ClientProgress;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleClientStatusProvider
{
    string ModuleKey { get; }

    Task<ClientProgressSnapshotDto?> BuildSnapshotAsync(
        Guid userId,
        Guid projectId,
        ClientProgressPublishChoices choices,
        IReadOnlyList<ClientProgressRevisionDto> revisions,
        DateTime publishedAt,
        CancellationToken cancellationToken = default);

    Task<ClientProgressWorkFacts?> GetWorkFactsAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default);
}
