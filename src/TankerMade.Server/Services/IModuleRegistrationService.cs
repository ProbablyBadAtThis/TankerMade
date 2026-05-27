namespace TankerMade.Server.Services;

public interface IModuleRegistrationService
{
    Task SyncDiscoveredModulesAsync(CancellationToken cancellationToken = default);
}
