using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleRecentWorkSummaryProvider
{
    string ModuleKey { get; }

    Task<IReadOnlyList<RecentWorkSummaryDto>> GetSummariesAsync(
        Guid userId,
        IReadOnlyList<RecentWorkItemRef> items,
        CancellationToken cancellationToken = default);
}
