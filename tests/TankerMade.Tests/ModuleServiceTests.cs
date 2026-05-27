using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Printing3D;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using Xunit;

namespace TankerMade.Tests;

public class ModuleServiceTests
{
    [Fact]
    public async Task ActivateAsync_marks_bundled_crafting_module_active_for_user()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);

        var activated = await service.ActivateAsync(CraftingModule.ModuleKey, user.Id);
        var activeModules = await service.GetActiveModulesAsync(user.Id);

        Assert.NotNull(activated);
        Assert.True(activated.IsActive);
        Assert.Contains(activeModules, module => module.ModuleKey == CraftingModule.ModuleKey);
    }

    [Fact]
    public async Task ActivateAsync_marks_bundled_printing_3d_module_active_for_user()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);

        var activated = await service.ActivateAsync(Printing3DModule.ModuleKey, user.Id);
        var activeModules = await service.GetActiveModulesAsync(user.Id);

        Assert.NotNull(activated);
        Assert.True(activated.IsActive);
        Assert.Contains(activeModules, module => module.ModuleKey == Printing3DModule.ModuleKey);
    }

    [Fact]
    public void BundledModuleCatalog_validate_accepts_current_module_contracts()
    {
        BundledModuleCatalog.Validate();

        var moduleKeys = BundledModuleCatalog.Registrations
            .Select(registration => registration.Module.ModuleKey)
            .ToList();

        Assert.Contains(CraftingModule.ModuleKey, moduleKeys);
        Assert.Contains(Printing3DModule.ModuleKey, moduleKeys);
        Assert.Equal(moduleKeys.Count, moduleKeys.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
