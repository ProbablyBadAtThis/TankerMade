using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Contracts.Services;

public interface IDashboardOverviewService
{
    Task<DashboardOverviewDto> GetOverviewAsync(
        Guid userId,
        string? moduleKey = null,
        CancellationToken cancellationToken = default);
}
