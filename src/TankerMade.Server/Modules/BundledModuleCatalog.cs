using TankerMade.Core.Modules;
using TankerMade.Modules.Crochet;
using TankerMade.Modules.Embroidery;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Printing3D;
using TankerMade.Modules.Quilting;
using TankerMade.Modules.Sewing;

namespace TankerMade.Server.Modules;

public static class BundledModuleCatalog
{
    public static readonly IReadOnlyList<ModuleDiscoveryRegistration> Registrations =
    [
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555552"),
            Printing3DModule.Instance),
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555553"),
            CrochetModule.Instance),
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555554"),
            EmbroideryModule.Instance),
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555555"),
            KnittingModule.Instance),
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555556"),
            QuiltingModule.Instance),
        new(
            Guid.Parse("55555555-5555-5555-5555-555555555557"),
            SewingModule.Instance)
    ];

    public static void Validate()
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in Registrations)
        {
            var module = registration.Module;

            if (string.IsNullOrWhiteSpace(module.ModuleKey))
            {
                throw new InvalidOperationException("Bundled module key is required.");
            }

            if (string.IsNullOrWhiteSpace(module.Name))
            {
                throw new InvalidOperationException($"Bundled module name is required for '{module.ModuleKey}'.");
            }

            if (string.IsNullOrWhiteSpace(module.Version))
            {
                throw new InvalidOperationException($"Bundled module version is required for '{module.ModuleKey}'.");
            }

            if (!keys.Add(module.ModuleKey.Trim()))
            {
                throw new InvalidOperationException($"Duplicate bundled module key detected: '{module.ModuleKey}'.");
            }
        }
    }
}
