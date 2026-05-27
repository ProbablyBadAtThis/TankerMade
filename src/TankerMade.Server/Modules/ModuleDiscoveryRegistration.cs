using TankerMade.Core.Modules;

namespace TankerMade.Server.Modules;

public sealed record ModuleDiscoveryRegistration(Guid Id, IModule Module);
