using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace TankerMade.Client.Services;

public enum AppNavigationScope
{
    None,
    Core,
    Knitting,
}

public sealed record AppNavItem(
    string Label,
    string Href,
    string Icon,
    NavLinkMatch Match = NavLinkMatch.Prefix);

public static class AppNavigation
{
    public static AppNavigationScope GetScope(string relativePath)
    {
        var path = Normalize(relativePath);

        if (path is "" or "login" or "register")
        {
            return AppNavigationScope.None;
        }

        if (path is "home" || path.StartsWith("home/", StringComparison.OrdinalIgnoreCase))
        {
            return AppNavigationScope.Core;
        }

        if (path.StartsWith("modules/knitting", StringComparison.OrdinalIgnoreCase))
        {
            return AppNavigationScope.Knitting;
        }

        return AppNavigationScope.None;
    }

    public static IReadOnlyList<AppNavItem> GetItems(AppNavigationScope scope)
        => scope switch
        {
            AppNavigationScope.Core =>
            [
                new("Dashboard", "/home", Icons.Material.Filled.Home, NavLinkMatch.All),
                new("Modules", "/home/modules", Icons.Material.Filled.Extension, NavLinkMatch.All),
            ],
            AppNavigationScope.Knitting =>
            [
                new("Home", "/modules/knitting", Icons.Material.Filled.Dashboard, NavLinkMatch.All),
                new("Projects", "/modules/knitting/projects", Icons.Material.Filled.Assignment, NavLinkMatch.Prefix),
                new("Patterns", "/modules/knitting/patterns", Icons.Material.Filled.MenuBook, NavLinkMatch.Prefix),
                new("Inventory", "/modules/knitting/inventory", Icons.Material.Filled.Inventory2, NavLinkMatch.Prefix),
                new("Kits", "/modules/knitting/kits", Icons.Material.Filled.Layers, NavLinkMatch.Prefix),
                new("Glossary", "/modules/knitting/glossary", Icons.Material.Filled.LibraryBooks, NavLinkMatch.All),
                new("Settings", "/modules/knitting/settings", Icons.Material.Filled.Settings, NavLinkMatch.All),
            ],
            _ => [],
        };

    public static AppNavItem? GetCoreDashboardItem(AppNavigationScope scope)
        => scope switch
        {
            AppNavigationScope.Knitting =>
                new("Dashboard", "/home", Icons.Material.Filled.Home, NavLinkMatch.All),
            _ => null,
        };

    private static string Normalize(string relativePath)
        => relativePath.Trim().Trim('/').ToLowerInvariant();
}
