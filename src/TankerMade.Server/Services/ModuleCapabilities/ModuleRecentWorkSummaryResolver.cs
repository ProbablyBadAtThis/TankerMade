using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleRecentWorkSummaryResolver
{
    IModuleRecentWorkSummaryProvider? Resolve(string moduleKey);
}

public class ModuleRecentWorkSummaryResolver : IModuleRecentWorkSummaryResolver
{
    private readonly IReadOnlyDictionary<string, IModuleRecentWorkSummaryProvider> _providers;

    public ModuleRecentWorkSummaryResolver(IEnumerable<IModuleRecentWorkSummaryProvider> providers)
    {
        _providers = providers.ToDictionary(
            provider => provider.ModuleKey,
            provider => provider,
            StringComparer.OrdinalIgnoreCase);
    }

    public IModuleRecentWorkSummaryProvider? Resolve(string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return null;
        }

        return _providers.TryGetValue(moduleKey.Trim(), out var provider)
            ? provider
            : null;
    }
}
