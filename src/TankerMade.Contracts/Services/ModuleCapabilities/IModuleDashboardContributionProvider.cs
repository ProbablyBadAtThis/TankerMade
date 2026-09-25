using TankerMade.Contracts.DTOs.Dashboard;

namespace TankerMade.Contracts.Services.ModuleCapabilities;

public interface IModuleDashboardContributionProvider
{
    string ModuleKey { get; }

    Task<ModuleDashboardContributionDto> GetContributionAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
