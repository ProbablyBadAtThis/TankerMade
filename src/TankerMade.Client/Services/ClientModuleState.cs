namespace TankerMade.Client.Services;

public class ClientModuleState
{
    private readonly TankerMadeApiClient _apiClient;
    private readonly HashSet<string> _activeModuleKeys = [];
    private readonly List<ClientModuleSummary> _availableModules =
    [
        new("printing-3d", "3D Printing", "Live module for 3D printing workflows and data.", false, "3D Printing", "modules/printing-3d", 200),
        new("crochet", "Crochet", "Live module for crochet workflows and data.", false, "Crochet", "modules/crochet", 300),
        new("embroidery", "Embroidery", "Live module for embroidery workflows and data.", false, "Embroidery", "modules/embroidery", 400),
        new("knitting", "Knitting", "Live module for knitting workflows and data.", false, "Knitting", "modules/knitting", 500),
        new("quilting", "Quilting", "Live module for quilting workflows and data.", false, "Quilting", "modules/quilting", 600),
        new("sewing", "Sewing", "Live module for sewing workflows and data.", false, "Sewing", "modules/sewing", 700)
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
        _availableModules.Add(new("printing-3d", "3D Printing", "Live module for 3D printing workflows and data.", false, "3D Printing", "modules/printing-3d", 200));
        _availableModules.Add(new("crochet", "Crochet", "Live module for crochet workflows and data.", false, "Crochet", "modules/crochet", 300));
        _availableModules.Add(new("embroidery", "Embroidery", "Live module for embroidery workflows and data.", false, "Embroidery", "modules/embroidery", 400));
        _availableModules.Add(new("knitting", "Knitting", "Live module for knitting workflows and data.", false, "Knitting", "modules/knitting", 500));
        _availableModules.Add(new("quilting", "Quilting", "Live module for quilting workflows and data.", false, "Quilting", "modules/quilting", 600));
        _availableModules.Add(new("sewing", "Sewing", "Live module for sewing workflows and data.", false, "Sewing", "modules/sewing", 700));
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
