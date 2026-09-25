using TankerMade.Core.Modules;

namespace TankerMade.Modules.Knitting;

public sealed class KnittingModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "knitting";
    public const string Name = "Knitting";
    public const string Version = "0.1.0";
    public const string Description = "Live module for knitting workflows and data.";
    public const string DefaultProjectThumbnailPath = "modules/knitting/default-project.svg";

    public static readonly KnittingModule Instance = new();

    private KnittingModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/knitting";
    int IModuleNavigation.NavigationOrder => 500;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
