using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleKitCapabilityResolver
{
    IModuleKitCapabilityHandler? Resolve(string moduleKey);
}

public class ModuleKitCapabilityResolver : IModuleKitCapabilityResolver
{
    private readonly IReadOnlyDictionary<string, IModuleKitCapabilityHandler> _handlers;

    public ModuleKitCapabilityResolver(IEnumerable<IModuleKitCapabilityHandler> handlers)
    {
        _handlers = handlers.ToDictionary(handler => handler.ModuleKey, handler => handler, StringComparer.OrdinalIgnoreCase);
    }

    public IModuleKitCapabilityHandler? Resolve(string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return null;
        }

        return _handlers.TryGetValue(moduleKey.Trim(), out var handler) ? handler : null;
    }
}
