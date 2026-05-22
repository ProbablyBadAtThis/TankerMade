using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Patterns;
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
        Assert.Equal(10, reloaded.Progress);
    }

    [Fact]
    public async Task CreateAsync_links_only_owned_patterns()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var patternService = new CraftingPatternService(context);
        var projectService = new CraftingProjectService(context);
        var ownedPattern = await patternService.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Owned Pattern"
        }, owner.Id);
        var otherPattern = await patternService.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Other Pattern"
        }, otherUser.Id);

        var created = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Linked Project",
            PatternId = ownedPattern.Id
        }, owner.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.CreateAsync(new CreateCraftingProjectDto
            {
                Name = "Cross Linked Project",
                PatternId = otherPattern.Id
            }, owner.Id));

        Assert.Equal(ownedPattern.Id, created.PatternId);
        Assert.Equal("The selected pattern is not available for this project.", rejected.Message);
    }

    [Fact]
    public async Task UpdateAsync_can_clear_linked_pattern()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var patternService = new CraftingPatternService(context);
        var projectService = new CraftingProjectService(context);
        var pattern = await patternService.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Linked Pattern"
        }, user.Id);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Linked Project",
            PatternId = pattern.Id
        }, user.Id);

        var updated = await projectService.UpdateAsync(new UpdateCraftingProjectDto
        {
            Id = project.Id,
            ClearPatternId = true
        }, user.Id);

        Assert.NotNull(updated);
        Assert.Null(updated.PatternId);
        Assert.Equal(string.Empty, updated.PatternName);
    }

    [Fact]
    public async Task CreateAsync_rejects_progress_outside_zero_to_one_hundred()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingProjectService(context);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateCraftingProjectDto
            {
                Name = "Too Far",
                Progress = 101
            }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateCraftingProjectDto
            {
                Name = "Too Low",
                Progress = -1
            }, user.Id));
    }
}
