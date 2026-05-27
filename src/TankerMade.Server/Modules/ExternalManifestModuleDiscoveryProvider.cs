using System.Text.Json;
using TankerMade.Core.Modules;

namespace TankerMade.Server.Modules;

public class ExternalManifestModuleDiscoveryProvider : IModuleDiscoveryProvider
{
    private readonly ModuleDiscoveryOptions _options;

    public ExternalManifestModuleDiscoveryProvider(ModuleDiscoveryOptions options)
    {
        _options = options;
    }

    public async Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ExternalManifestDirectory))
        {
            return [];
        }

        var directory = _options.ExternalManifestDirectory.Trim();
        if (!Path.IsPathRooted(directory))
        {
            directory = Path.GetFullPath(directory);
        }

        if (!Directory.Exists(directory))
        {
            return [];
        }

        var files = Directory.GetFiles(directory, "*.module.json", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var modules = new List<ModuleDiscoveryRegistration>(files.Count);
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await using var stream = File.OpenRead(file);
            var manifest = await JsonSerializer.DeserializeAsync<ExternalModuleManifest>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (manifest == null)
            {
                throw new InvalidOperationException($"Module manifest '{file}' is empty or invalid JSON.");
            }

            modules.Add(new ModuleDiscoveryRegistration(
                manifest.Id,
                new ExternalDiscoveredModule(manifest)));
        }

        return modules;
    }

    private sealed class ExternalDiscoveredModule : IModule, IModuleNavigation, IModulePackaging
    {
        private readonly ExternalModuleManifest _manifest;

        public ExternalDiscoveredModule(ExternalModuleManifest manifest)
        {
            _manifest = manifest;
        }

        public string ModuleKey => _manifest.ModuleKey;
        public string Name => _manifest.Name;
        public string Description => _manifest.Description ?? string.Empty;
        public string Version => _manifest.Version;
        public bool IsBundled => false;

        public string NavigationLabel => string.IsNullOrWhiteSpace(_manifest.Navigation?.Label) ? Name : _manifest.Navigation.Label;
        public string NavigationRoute => _manifest.Navigation?.Route ?? string.Empty;
        public int NavigationOrder => _manifest.Navigation?.Order ?? 1000;

        public string ManifestVersion => _manifest.Packaging.ManifestVersion;
        public string MinHostVersion => _manifest.Packaging.MinHostVersion;
        public string? MaxHostVersion => _manifest.Packaging.MaxHostVersion;
    }
}

public sealed class ExternalModuleManifest
{
    public Guid Id { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public ExternalModuleNavigationManifest? Navigation { get; set; }
    public ExternalModulePackagingManifest Packaging { get; set; } = new();
}

public sealed class ExternalModuleNavigationManifest
{
    public string Label { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int Order { get; set; }
}

public sealed class ExternalModulePackagingManifest
{
    public string ManifestVersion { get; set; } = string.Empty;
    public string MinHostVersion { get; set; } = string.Empty;
    public string? MaxHostVersion { get; set; }
}
