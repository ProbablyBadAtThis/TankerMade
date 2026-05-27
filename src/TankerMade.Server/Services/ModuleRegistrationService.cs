using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Entities;
using TankerMade.Core.Modules;
using TankerMade.Server.Data;
using TankerMade.Server.Modules;

namespace TankerMade.Server.Services;

public class ModuleRegistrationService : IModuleRegistrationService
{
    private readonly TankerMadeDbContext _context;
    private readonly IReadOnlyList<IModuleDiscoveryProvider> _discoveryProviders;

    public ModuleRegistrationService(
        TankerMadeDbContext context,
        IEnumerable<IModuleDiscoveryProvider> discoveryProviders)
    {
        _context = context;
        _discoveryProviders = discoveryProviders.ToList();
    }

    public async Task SyncDiscoveredModulesAsync(CancellationToken cancellationToken = default)
    {
        var discovered = new List<ModuleDiscoveryRegistration>();
        foreach (var provider in _discoveryProviders)
        {
            var results = await provider.DiscoverAsync(cancellationToken);
            discovered.AddRange(results);
        }

        ValidateDiscovered(discovered);

        var registrationById = discovered.ToDictionary(r => r.Id);
        var registrationIds = discovered.Select(r => r.Id).ToHashSet();

        var existingBundled = await _context.ModuleDefinitions
            .Where(definition => definition.IsBundled)
            .ToListAsync();

        foreach (var definition in existingBundled)
        {
            if (!registrationById.TryGetValue(definition.Id, out var registration))
            {
                definition.IsBundled = false;
                continue;
            }

            var module = registration.Module;
            definition.ModuleKey = module.ModuleKey.Trim();
            definition.Name = module.Name.Trim();
            definition.Description = module.Description.Trim();
            definition.Version = module.Version.Trim();
            definition.IsBundled = module.IsBundled;
        }

        var existingIds = existingBundled.Select(definition => definition.Id).ToHashSet();

        foreach (var registration in discovered)
        {
            if (existingIds.Contains(registration.Id))
            {
                continue;
            }

            var module = registration.Module;
            _context.ModuleDefinitions.Add(new ModuleDefinition(
                registration.Id,
                module.ModuleKey.Trim(),
                module.Name.Trim(),
                module.Description.Trim(),
                module.Version.Trim(),
                module.IsBundled));
        }

        var staleActivations = await _context.UserModuleActivations
            .Where(activation => !registrationIds.Contains(activation.ModuleDefinitionId) && activation.IsActive)
            .ToListAsync();

        foreach (var activation in staleActivations)
        {
            activation.Deactivate();
        }

        await _context.SaveChangesAsync();
    }

    private static void ValidateDiscovered(IReadOnlyList<ModuleDiscoveryRegistration> discovered)
    {
        var ids = new HashSet<Guid>();
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in discovered)
        {
            if (!ids.Add(registration.Id))
            {
                throw new InvalidOperationException($"Duplicate discovered module id detected: '{registration.Id}'.");
            }

            var module = registration.Module;
            if (string.IsNullOrWhiteSpace(module.ModuleKey))
            {
                throw new InvalidOperationException("Discovered module key is required.");
            }

            var normalizedKey = module.ModuleKey.Trim();
            if (!keys.Add(normalizedKey))
            {
                throw new InvalidOperationException($"Duplicate discovered module key detected: '{normalizedKey}'.");
            }

            ValidatePackaging(module);
        }
    }

    private static void ValidatePackaging(IModule module)
    {
        if (module is not IModulePackaging packaging)
        {
            throw new InvalidOperationException(
                $"Discovered module '{module.ModuleKey}' must declare packaging metadata.");
        }

        if (!string.Equals(
            packaging.ManifestVersion,
            HostRuntimeInfo.SupportedManifestVersion,
            StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Discovered module '{module.ModuleKey}' has unsupported manifest version '{packaging.ManifestVersion}'.");
        }

        if (!Version.TryParse(HostRuntimeInfo.HostVersion, out var hostVersion))
        {
            throw new InvalidOperationException("Host version is not a valid semantic version.");
        }

        if (!Version.TryParse(packaging.MinHostVersion, out var minHostVersion))
        {
            throw new InvalidOperationException(
                $"Discovered module '{module.ModuleKey}' has invalid MinHostVersion '{packaging.MinHostVersion}'.");
        }

        if (hostVersion < minHostVersion)
        {
            throw new InvalidOperationException(
                $"Discovered module '{module.ModuleKey}' requires host >= {packaging.MinHostVersion}.");
        }

        if (!string.IsNullOrWhiteSpace(packaging.MaxHostVersion))
        {
            if (!Version.TryParse(packaging.MaxHostVersion, out var maxHostVersion))
            {
                throw new InvalidOperationException(
                    $"Discovered module '{module.ModuleKey}' has invalid MaxHostVersion '{packaging.MaxHostVersion}'.");
            }

            if (hostVersion > maxHostVersion)
            {
                throw new InvalidOperationException(
                    $"Discovered module '{module.ModuleKey}' requires host <= {packaging.MaxHostVersion}.");
            }
        }
    }
}
