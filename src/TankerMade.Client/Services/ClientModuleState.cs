namespace TankerMade.Client.Services;

public class ClientModuleState
{
    private readonly HashSet<string> _activeModuleKeys = [];

    public event Action? Changed;

    public IReadOnlyList<ClientModuleSummary> AvailableModules { get; } =
    [
        new("crafting", "Crafting", "Pattern-based maker workflows supplied by the reference module.")
    ];

    public bool IsActive(string moduleKey)
    {
        return _activeModuleKeys.Contains(moduleKey);
    }

    public void Activate(string moduleKey)
    {
        if (_activeModuleKeys.Add(moduleKey))
        {
            Changed?.Invoke();
        }
    }

    public void Deactivate(string moduleKey)
    {
        if (_activeModuleKeys.Remove(moduleKey))
        {
            Changed?.Invoke();
        }
    }
}

public record ClientModuleSummary(string ModuleKey, string Name, string Description);
