using TankerMade.Core.Modules;

namespace TankerMade.Modules.Sewing;

public sealed class SewingModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "sewing";
    public const string Name = "Sewing";
    public const string Version = "0.1.0";
    public const string Description = "Live module for sewing workflows and data.";

    public static readonly SewingModule Instance = new();

    private SewingModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/sewing";
    int IModuleNavigation.NavigationOrder => 700;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
