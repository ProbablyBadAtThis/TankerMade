using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleDashboardContributionResolver
{
    IModuleDashboardContributionProvider? Resolve(string moduleKey);
}

public class ModuleDashboardContributionResolver : IModuleDashboardContributionResolver
{
    private readonly IReadOnlyDictionary<string, IModuleDashboardContributionProvider> _providers;

    public ModuleDashboardContributionResolver(IEnumerable<IModuleDashboardContributionProvider> providers)
    {
        _providers = providers.ToDictionary(
            provider => provider.ModuleKey,
            StringComparer.OrdinalIgnoreCase);
    }

    public IModuleDashboardContributionProvider? Resolve(string moduleKey)
        => _providers.TryGetValue(moduleKey, out var provider) ? provider : null;
}
