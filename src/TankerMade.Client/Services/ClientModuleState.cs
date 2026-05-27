namespace TankerMade.Client.Services;

public class ClientModuleState
{
    private readonly TankerMadeApiClient _apiClient;
    private readonly HashSet<string> _activeModuleKeys = [];
    private readonly List<ClientModuleSummary> _availableModules =
    [
        new("crafting", "Crafting", "Pattern-based maker workflows supplied by the reference module.", false, "Crafting", "modules/crafting", 100)
    ];

    public ClientModuleState(TankerMadeApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public event Action? Changed;

    public IReadOnlyList<ClientModuleSummary> AvailableModules => _availableModules;

    public async Task RefreshAsync()
    {
        var modules = await _apiClient.GetModulesAsync();
        _availableModules.Clear();
        _activeModuleKeys.Clear();

        foreach (var module in modules.OrderBy(module => module.Name))
        {
            _availableModules.Add(new ClientModuleSummary(
                module.ModuleKey,
                module.Name,
                module.Description,
                module.IsActive,
                module.NavigationLabel,
                module.NavigationRoute,
                module.NavigationOrder));

            if (module.IsActive)
            {
                _activeModuleKeys.Add(module.ModuleKey);
            }
        }

        Changed?.Invoke();
    }

    public bool IsActive(string moduleKey)
    {
        return _activeModuleKeys.Contains(moduleKey);
    }

    public async Task ActivateAsync(string moduleKey)
    {
        var module = await _apiClient.ActivateModuleAsync(moduleKey);
        if (module == null)
        {
            return;
        }

        SetModuleState(module.ModuleKey, module.IsActive);
        Changed?.Invoke();
    }

    public async Task DeactivateAsync(string moduleKey)
    {
        await _apiClient.DeactivateModuleAsync(moduleKey);
        SetModuleState(moduleKey, false);
        Changed?.Invoke();
    }

    public void Reset()
    {
        _activeModuleKeys.Clear();
        _availableModules.Clear();
        _availableModules.Add(new("crafting", "Crafting", "Pattern-based maker workflows supplied by the reference module.", false, "Crafting", "modules/crafting", 100));
        Changed?.Invoke();
    }

    private void SetModuleState(string moduleKey, bool isActive)
    {
        if (isActive)
        {
            _activeModuleKeys.Add(moduleKey);
        }
        else
        {
            _activeModuleKeys.Remove(moduleKey);
        }

        var index = _availableModules.FindIndex(module => module.ModuleKey == moduleKey);
        if (index >= 0)
        {
            var module = _availableModules[index];
            _availableModules[index] = module with { IsActive = isActive };
        }
    }
}

public record ClientModuleSummary(
    string ModuleKey,
    string Name,
    string Description,
    bool IsActive,
    string NavigationLabel,
    string NavigationRoute,
    int NavigationOrder);
