using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleInventoryCapabilityResolver
{
    IModuleInventoryCapabilityHandler? Resolve(string moduleKey);
}

public class ModuleInventoryCapabilityResolver : IModuleInventoryCapabilityResolver
{
    private readonly IReadOnlyDictionary<string, IModuleInventoryCapabilityHandler> _handlers;

    public ModuleInventoryCapabilityResolver(IEnumerable<IModuleInventoryCapabilityHandler> handlers)
    {
        _handlers = handlers.ToDictionary(
            handler => handler.ModuleKey,
            handler => handler,
            StringComparer.OrdinalIgnoreCase);
    }

    public IModuleInventoryCapabilityHandler? Resolve(string moduleKey)
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
