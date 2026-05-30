using TankerMade.Core.Modules;

namespace TankerMade.Modules.Quilting;

public sealed class QuiltingModule : IModule, IModuleNavigation, IModulePackaging
{
    public const string ModuleKey = "quilting";
    public const string Name = "Quilting";
    public const string Version = "0.1.0";
    public const string Description = "Live module for quilting workflows and data.";

    public static readonly QuiltingModule Instance = new();

    private QuiltingModule()
    {
    }

    string IModule.ModuleKey => ModuleKey;
    string IModule.Name => Name;
    string IModule.Description => Description;
    string IModule.Version => Version;
    bool IModule.IsBundled => true;

    string IModuleNavigation.NavigationLabel => Name;
    string IModuleNavigation.NavigationRoute => "modules/quilting";
    int IModuleNavigation.NavigationOrder => 600;

    string IModulePackaging.ManifestVersion => "1";
    string IModulePackaging.MinHostVersion => "1.0.0";
    string? IModulePackaging.MaxHostVersion => null;
}
