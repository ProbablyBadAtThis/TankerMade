using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleProjectCapabilityResolver
{
    IModuleProjectCapabilityHandler? Resolve(string moduleKey);
}

public class ModuleProjectCapabilityResolver : IModuleProjectCapabilityResolver
{
    private readonly IReadOnlyDictionary<string, IModuleProjectCapabilityHandler> _handlers;

    public ModuleProjectCapabilityResolver(IEnumerable<IModuleProjectCapabilityHandler> handlers)
    {
        _handlers = handlers.ToDictionary(
            handler => handler.ModuleKey,
            handler => handler,
            StringComparer.OrdinalIgnoreCase);
    }

    public IModuleProjectCapabilityHandler? Resolve(string moduleKey)
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
