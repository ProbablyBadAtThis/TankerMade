using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModulePatternCapabilityResolver
{
    IModulePatternCapabilityHandler? Resolve(string moduleKey);
}

public class ModulePatternCapabilityResolver : IModulePatternCapabilityResolver
{
    private readonly IReadOnlyDictionary<string, IModulePatternCapabilityHandler> _handlers;

    public ModulePatternCapabilityResolver(IEnumerable<IModulePatternCapabilityHandler> handlers)
    {
        _handlers = handlers.ToDictionary(
            handler => handler.ModuleKey,
            handler => handler,
            StringComparer.OrdinalIgnoreCase);
    }

    public IModulePatternCapabilityHandler? Resolve(string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return null;
        }

        return _handlers.TryGetValue(moduleKey.Trim(), out var handler)
            ? handler
            : null;
    }
}
