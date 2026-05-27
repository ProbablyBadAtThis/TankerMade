using TankerMade.Core.Modules;

namespace TankerMade.Modules.Crafting;

public sealed class CraftingModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "crafting";
    public const string Name = "Crafting";
    public const string Version = "0.1.0";
    public const string Description = "Reference maker module for pattern-based crafting workflows.";

    public static readonly CraftingModule Instance = new();

    private CraftingModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/crafting";
    int IModuleNavigation.NavigationOrder => 100;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
