using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Server.Services.Crafting;
using Xunit;

namespace TankerMade.Tests;

public class CraftingProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_persists_project_in_crafting_module_table()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingProjectService(context);

        var created = await service.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Reference Project",
            Description = "A module-owned project.",
            Difficulty = 1,
            Progress = 10
        }, user.Id);

        var reloaded = await service.GetByIdAsync(created.Id, user.Id);

        Assert.NotNull(reloaded);
        Assert.Equal("Reference Project", reloaded.Name);
        Assert.Equal("reference-project", reloaded.Slug);
        Assert.Equal(10m, reloaded.Progress);
    }
}
