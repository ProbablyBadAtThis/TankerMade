using TankerMade.Contracts.Services.ModuleCapabilities;

namespace TankerMade.Server.Services.ModuleCapabilities;

public interface IModuleSettingsCapabilityResolver
{
    IModuleSettingsCapabilityHandler? Resolve(string moduleKey);
}

public class ModuleSettingsCapabilityResolver : IModuleSettingsCapabilityResolver
{
    private readonly IReadOnlyDictionary<string, IModuleSettingsCapabilityHandler> _handlers;

    public ModuleSettingsCapabilityResolver(IEnumerable<IModuleSettingsCapabilityHandler> handlers)
    {
        _handlers = handlers.ToDictionary(handler => handler.ModuleKey, handler => handler, StringComparer.OrdinalIgnoreCase);
    }

    public IModuleSettingsCapabilityHandler? Resolve(string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return null;
        }

        return _handlers.TryGetValue(moduleKey.Trim(), out var handler) ? handler : null;
    }
}
