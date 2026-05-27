namespace TankerMade.Core.Modules;

public interface IModule
{
    string ModuleKey { get; }
    string Name { get; }
    string Description { get; }
    string Version { get; }
    bool IsBundled { get; }
}
