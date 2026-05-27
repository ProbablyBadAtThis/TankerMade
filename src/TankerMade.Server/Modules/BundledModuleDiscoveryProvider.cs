namespace TankerMade.Server.Modules;

public class BundledModuleDiscoveryProvider : IModuleDiscoveryProvider
{
    public Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        BundledModuleCatalog.Validate();
        return Task.FromResult(BundledModuleCatalog.Registrations);
    }
}
