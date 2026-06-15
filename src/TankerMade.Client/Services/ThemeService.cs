using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace TankerMade.Client.Services;

public class ThemeService
{
    private const string ThemeKey = "tankermade.theme";
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;

    public ThemeService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
    }

    public string CurrentTheme { get; private set; } = "dark";

    public bool IsDarkMode => string.Equals(CurrentTheme, "dark", StringComparison.OrdinalIgnoreCase);

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        var stored = await _localStorage.GetItemAsync<string>(ThemeKey);
        CurrentTheme = string.Equals(stored, "light", StringComparison.OrdinalIgnoreCase) ? "light" : "dark";
        await ApplyAsync(CurrentTheme);
        Changed?.Invoke();
    }

    public async Task ToggleAsync()
    {
        CurrentTheme = CurrentTheme == "dark" ? "light" : "dark";
        await _localStorage.SetItemAsync(ThemeKey, CurrentTheme);
        await ApplyAsync(CurrentTheme);
        Changed?.Invoke();
    }

    public async Task SetDarkModeAsync(bool isDarkMode)
    {
        var theme = isDarkMode ? "dark" : "light";
        if (string.Equals(CurrentTheme, theme, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentTheme = theme;
        await _localStorage.SetItemAsync(ThemeKey, CurrentTheme);
        await ApplyAsync(CurrentTheme);
        Changed?.Invoke();
    }

    private async Task ApplyAsync(string theme)
    {
        await _jsRuntime.InvokeVoidAsync("tankerMadeTheme.set", theme);
    }
}
