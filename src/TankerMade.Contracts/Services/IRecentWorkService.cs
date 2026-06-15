using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Contracts.Services;

public interface IRecentWorkService
{
    Task RecordAccessAsync(Guid userId, RecordRecentWorkRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecentWorkSummaryDto>> GetRecentAsync(
        Guid userId,
        int limit = 5,
        CancellationToken cancellationToken = default);
}
