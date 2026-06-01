using TankerMade.Core.Entities;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Printing3D;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using Xunit;

namespace TankerMade.Tests;

public class ModuleRegistrationServiceTests
{
    [Fact]
    public async Task SyncDiscoveredModulesAsync_adds_missing_bundled_modules()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        context.ModuleDefinitions.RemoveRange(context.ModuleDefinitions);
        await context.SaveChangesAsync();

        var service = new ModuleRegistrationService(context, [new BundledModuleDiscoveryProvider()]);
        await service.SyncDiscoveredModulesAsync();

        var moduleKeys = context.ModuleDefinitions
            .Select(definition => definition.ModuleKey)
            .ToList();

        Assert.Contains(KnittingModule.ModuleKey, moduleKeys);
        Assert.Contains(Printing3DModule.ModuleKey, moduleKeys);
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_updates_bundled_module_metadata()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var knitting = context.ModuleDefinitions.Single(definition => definition.ModuleKey == KnittingModule.ModuleKey);
        knitting.Name = "Old Name";
        knitting.Version = "0.0.1";
        await context.SaveChangesAsync();

        var service = new ModuleRegistrationService(context, [new BundledModuleDiscoveryProvider()]);
        await service.SyncDiscoveredModulesAsync();

        var refreshed = context.ModuleDefinitions.Single(definition => definition.Id == knitting.Id);
        Assert.Equal(KnittingModule.Name, refreshed.Name);
        Assert.Equal(KnittingModule.Version, refreshed.Version);
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_deactivates_stale_bundled_module_activations()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);

        var staleModule = new ModuleDefinition(
            Guid.NewGuid(),
            "stale-module",
            "Stale Module",
            "No longer bundled",
            "1.0.0",
            true);
        context.ModuleDefinitions.Add(staleModule);
        await context.SaveChangesAsync();

        var activation = new UserModuleActivation(Guid.NewGuid(), user.Id, staleModule.Id);
        context.UserModuleActivations.Add(activation);
        await context.SaveChangesAsync();

        var service = new ModuleRegistrationService(context, [new BundledModuleDiscoveryProvider()]);
        await service.SyncDiscoveredModulesAsync();

        var refreshedActivation = context.UserModuleActivations.Single(a => a.Id == activation.Id);
        var refreshedModule = context.ModuleDefinitions.Single(definition => definition.Id == staleModule.Id);

        Assert.False(refreshedActivation.IsActive);
        Assert.False(refreshedModule.IsBundled);
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_throws_for_duplicate_discovered_keys()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var duplicateKey = KnittingModule.ModuleKey;
        var provider = new DuplicateKeyDiscoveryProvider(duplicateKey);
        var service = new ModuleRegistrationService(context, [provider]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SyncDiscoveredModulesAsync());
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_throws_for_incompatible_host_version()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var provider = new IncompatibleHostVersionDiscoveryProvider();
        var service = new ModuleRegistrationService(context, [provider]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SyncDiscoveredModulesAsync());
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_throws_when_packaging_metadata_is_missing()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var provider = new MissingPackagingDiscoveryProvider();
        var service = new ModuleRegistrationService(context, [provider]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SyncDiscoveredModulesAsync());
    }

    [Fact]
    public async Task SyncDiscoveredModulesAsync_registers_external_manifest_modules()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var directory = Path.Combine(Path.GetTempPath(), $"tankermade-modules-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            var manifestPath = Path.Combine(directory, "external.module.json");
            await File.WriteAllTextAsync(manifestPath, """
            {
              "id": "c537a31f-a0a7-4d5f-9f66-a2f760f7dfbe",
              "moduleKey": "external-sample",
              "name": "External Sample",
              "description": "External discovery test",
              "version": "0.0.1",
              "navigation": {
                "label": "External Sample",
                "route": "modules/external-sample",
                "order": 900
              },
              "packaging": {
                "manifestVersion": "1",
                "minHostVersion": "1.0.0",
                "maxHostVersion": null
              }
            }
            """);

            var externalProvider = new ExternalManifestModuleDiscoveryProvider(new ModuleDiscoveryOptions
            {
                ExternalManifestDirectory = directory
            });

            var service = new ModuleRegistrationService(
                context,
                [new BundledModuleDiscoveryProvider(), externalProvider]);

            await service.SyncDiscoveredModulesAsync();

            var external = context.ModuleDefinitions.SingleOrDefault(definition => definition.ModuleKey == "external-sample");
            Assert.NotNull(external);
            Assert.False(external!.IsBundled);
            Assert.Equal("External Sample", external.Name);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    private sealed class DuplicateKeyDiscoveryProvider : IModuleDiscoveryProvider
    {
        private readonly string _moduleKey;

        public DuplicateKeyDiscoveryProvider(string moduleKey)
        {
            _moduleKey = moduleKey;
        }

        public Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ModuleDiscoveryRegistration> result =
            [
                new(
                    Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    new TestModule(_moduleKey, "One")),
                new(
                    Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    new TestModule(_moduleKey, "Two"))
            ];

            return Task.FromResult(result);
        }
    }

    private sealed class TestModule : TankerMade.Core.Modules.IModule, TankerMade.Core.Modules.IModulePackaging
    {
        public TestModule(string moduleKey, string name)
        {
            ModuleKey = moduleKey;
            Name = name;
        }

        public string ModuleKey { get; }
        public string Name { get; }
        public string Description => string.Empty;
        public string Version => "0.0.0-test";
        public bool IsBundled => true;
        public string ManifestVersion => "1";
        public string MinHostVersion => "1.0.0";
        public string? MaxHostVersion => null;
    }

    private sealed class IncompatibleHostVersionDiscoveryProvider : IModuleDiscoveryProvider
    {
        public Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ModuleDiscoveryRegistration> result =
            [
                new(
                    Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    new IncompatibleModule())
            ];

            return Task.FromResult(result);
        }
    }

    private sealed class IncompatibleModule : TankerMade.Core.Modules.IModule, TankerMade.Core.Modules.IModulePackaging
    {
        public string ModuleKey => "incompatible-module";
        public string Name => "Incompatible Module";
        public string Description => string.Empty;
        public string Version => "0.0.0-test";
        public bool IsBundled => true;
        public string ManifestVersion => "1";
        public string MinHostVersion => "99.0.0";
        public string? MaxHostVersion => null;
    }

    private sealed class MissingPackagingDiscoveryProvider : IModuleDiscoveryProvider
    {
        public Task<IReadOnlyList<ModuleDiscoveryRegistration>> DiscoverAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ModuleDiscoveryRegistration> result =
            [
                new(
                    Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1"),
                    new MissingPackagingModule())
            ];

            return Task.FromResult(result);
        }
    }

    private sealed class MissingPackagingModule : TankerMade.Core.Modules.IModule
    {
        public string ModuleKey => "missing-packaging-module";
        public string Name => "Missing Packaging Module";
        public string Description => string.Empty;
        public string Version => "0.0.0-test";
        public bool IsBundled => true;
    }
}
