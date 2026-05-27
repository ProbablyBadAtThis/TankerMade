namespace TankerMade.Server.Modules;

public interface IModuleDiscoveryProvider
{
    Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default);
}
