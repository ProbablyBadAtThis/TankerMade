using TankerMade.Core.Modules;

namespace TankerMade.Modules.Embroidery;

public sealed class EmbroideryModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "embroidery";
    public const string Name = "Embroidery";
    public const string Version = "0.1.0";
    public const string Description = "Live module for embroidery workflows and data.";

    public static readonly EmbroideryModule Instance = new();

    private EmbroideryModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/embroidery";
    int IModuleNavigation.NavigationOrder => 400;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
