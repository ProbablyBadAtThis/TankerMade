using TankerMade.Core.Modules;

namespace TankerMade.Modules.Printing3D;

public sealed class Printing3DModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "printing-3d";
    public const string Name = "3D Printing";
    public const string Version = "0.1.0";
    public const string Description = "Live module for 3D printing workflows and data.";

    public static readonly Printing3DModule Instance = new();

    private Printing3DModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/printing-3d";
    int IModuleNavigation.NavigationOrder => 200;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
