using TankerMade.Server.Modules;
using Xunit;

namespace TankerMade.Tests;

public class ExternalManifestModuleDiscoveryProviderTests
{
    [Fact]
    public async Task DiscoverAsync_returns_external_modules_from_manifest_directory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"tankermade-modules-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            var manifestPath = Path.Combine(directory, "sample.module.json");
            await File.WriteAllTextAsync(manifestPath, """
            {
              "id": "d2cc98bc-4f73-42a8-9ea8-661be620b8d8",
              "moduleKey": "sample-module",
              "name": "Sample Module",
              "description": "External sample",
              "version": "0.0.1",
              "navigation": {
                "label": "Sample",
                "route": "modules/sample",
                "order": 500
              },
              "packaging": {
                "manifestVersion": "1",
                "minHostVersion": "1.0.0",
                "maxHostVersion": null
              }
            }
            """);

            var provider = new ExternalManifestModuleDiscoveryProvider(new ModuleDiscoveryOptions
            {
                ExternalManifestDirectory = directory
            });

            var discovered = await provider.DiscoverAsync();

            Assert.Single(discovered);
            Assert.Equal("sample-module", discovered[0].Module.ModuleKey);
            Assert.False(discovered[0].Module.IsBundled);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    [Fact]
    public async Task DiscoverAsync_returns_empty_when_directory_is_missing()
    {
        var provider = new ExternalManifestModuleDiscoveryProvider(new ModuleDiscoveryOptions
        {
            ExternalManifestDirectory = Path.Combine(Path.GetTempPath(), $"tankermade-missing-{Guid.NewGuid():N}")
        });

        var discovered = await provider.DiscoverAsync();

        Assert.Empty(discovered);
    }

    [Fact]
    public async Task DiscoverAsync_throws_when_manifest_json_is_invalid()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"tankermade-modules-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            var manifestPath = Path.Combine(directory, "broken.module.json");
            await File.WriteAllTextAsync(manifestPath, "{ not-json }");

            var provider = new ExternalManifestModuleDiscoveryProvider(new ModuleDiscoveryOptions
            {
                ExternalManifestDirectory = directory
            });

            await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => provider.DiscoverAsync());
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
