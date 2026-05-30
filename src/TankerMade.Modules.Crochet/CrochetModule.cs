using TankerMade.Core.Modules;

namespace TankerMade.Modules.Crochet;

public sealed class CrochetModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "crochet";
    public const string Name = "Crochet";
    public const string Version = "0.1.0";
    public const string Description = "Live module for crochet workflows and data.";

    public static readonly CrochetModule Instance = new();

    private CrochetModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/crochet";
    int IModuleNavigation.NavigationOrder => 300;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
