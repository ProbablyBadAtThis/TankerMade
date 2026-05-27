using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Inventory;
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

    [Fact]
    public async Task SetStepProgressAsync_tracks_completion_for_linked_pattern_steps()
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
            Name = "Tracked Pattern"
        }, user.Id);
        var piece = await patternService.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Main"
        }, user.Id);

        Assert.NotNull(piece);

        var firstStep = await patternService.AddStepAsync(pattern.Id, piece.Id, new CreateCraftingPatternStepDto
        {
            RangeStart = 1,
            RangeEnd = 1,
            Label = "Start",
            Instructions = "Start here."
        }, user.Id);

        Assert.NotNull(firstStep);

        await patternService.AddStepAsync(pattern.Id, piece.Id, new CreateCraftingPatternStepDto
        {
            RangeStart = 2,
            RangeEnd = 2,
            Label = "Continue",
            Instructions = "Keep going."
        }, user.Id);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Tracked Project",
            PatternId = pattern.Id
        }, user.Id);

        var checkedProject = await projectService.SetStepProgressAsync(
            project.Id,
            firstStep.Id,
            new UpdateCraftingProjectStepProgressDto { IsComplete = true },
            user.Id);
        var uncheckedProject = await projectService.SetStepProgressAsync(
            project.Id,
            firstStep.Id,
            new UpdateCraftingProjectStepProgressDto { IsComplete = false },
            user.Id);

        Assert.NotNull(checkedProject);
        Assert.Equal(1, checkedProject.CompletedStepCount);
        Assert.Equal(2, checkedProject.TotalStepCount);
        Assert.Contains(checkedProject.StepProgress, progress => progress.PatternStepId == firstStep.Id && progress.IsComplete);
        Assert.NotNull(uncheckedProject);
        Assert.Equal(0, uncheckedProject.CompletedStepCount);
        Assert.Equal(2, uncheckedProject.TotalStepCount);
        Assert.Empty(uncheckedProject.StepProgress);
    }

    [Fact]
    public async Task SetStepProgressAsync_rejects_steps_outside_linked_pattern()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var patternService = new CraftingPatternService(context);
        var projectService = new CraftingProjectService(context);
        var linkedPattern = await patternService.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Linked Pattern"
        }, user.Id);
        var linkedPiece = await patternService.AddPieceAsync(linkedPattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Linked Piece"
        }, user.Id);
        var otherPattern = await patternService.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Other Pattern"
        }, user.Id);
        var otherPiece = await patternService.AddPieceAsync(otherPattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Other Piece"
        }, user.Id);

        Assert.NotNull(linkedPiece);
        Assert.NotNull(otherPiece);

        await patternService.AddStepAsync(linkedPattern.Id, linkedPiece.Id, new CreateCraftingPatternStepDto
        {
            Label = "Allowed",
            Instructions = "This one belongs."
        }, user.Id);
        var otherStep = await patternService.AddStepAsync(otherPattern.Id, otherPiece.Id, new CreateCraftingPatternStepDto
        {
            Label = "Rejected",
            Instructions = "This one does not."
        }, user.Id);

        Assert.NotNull(otherStep);

        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Linked Project",
            PatternId = linkedPattern.Id
        }, user.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.SetStepProgressAsync(
                project.Id,
                otherStep.Id,
                new UpdateCraftingProjectStepProgressDto { IsComplete = true },
                user.Id));

        Assert.Equal("The selected step is not available for this project.", rejected.Message);
    }

    [Fact]
    public async Task AddInventoryLinkAsync_links_owned_inventory_to_project()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var projectService = new CraftingProjectService(context);
        var inventoryService = new CraftingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var yarn = await inventoryService.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Blue",
            Skeins = 2
        }, user.Id);

        var linked = await projectService.AddInventoryLinkAsync(project.Id, new CreateCraftingProjectInventoryLinkDto
        {
            InventoryItemType = "yarn",
            InventoryItemId = yarn.Id,
            QuantityPlanned = 1.5m,
            Notes = "Main body"
        }, user.Id);

        Assert.NotNull(linked);
        Assert.Single(linked.InventoryLinks);
        Assert.Equal("yarn", linked.InventoryLinks[0].InventoryItemType);
        Assert.Equal(yarn.Id, linked.InventoryLinks[0].InventoryItemId);
        Assert.Equal("Acme Yarn - Blue", linked.InventoryLinks[0].InventoryItemName);
        Assert.Equal(1.5m, linked.InventoryLinks[0].QuantityPlanned);
    }

    [Fact]
    public async Task AddInventoryLinkAsync_updates_existing_link_for_same_project_item()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var projectService = new CraftingProjectService(context);
        var inventoryService = new CraftingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var tool = await inventoryService.CreateOrMergeToolAsync(new CreateCraftingToolInventoryItemDto
        {
            BrandName = "Acme",
            TypeName = "Hook",
            Quantity = 1
        }, user.Id);

        await projectService.AddInventoryLinkAsync(project.Id, new CreateCraftingProjectInventoryLinkDto
        {
            InventoryItemType = "tool",
            InventoryItemId = tool.Id,
            QuantityPlanned = 1,
            Notes = "Original"
        }, user.Id);
        var updated = await projectService.AddInventoryLinkAsync(project.Id, new CreateCraftingProjectInventoryLinkDto
        {
            InventoryItemType = "tool",
            InventoryItemId = tool.Id,
            QuantityPlanned = 2,
            Notes = "Updated"
        }, user.Id);

        Assert.NotNull(updated);
        Assert.Single(updated.InventoryLinks);
        Assert.Equal(2, updated.InventoryLinks[0].QuantityPlanned);
        Assert.Equal("Updated", updated.InventoryLinks[0].Notes);
    }

    [Fact]
    public async Task AddInventoryLinkAsync_rejects_cross_user_inventory()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var projectService = new CraftingProjectService(context);
        var inventoryService = new CraftingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Supply Project"
        }, owner.Id);
        var otherNotion = await inventoryService.CreateOrMergeNotionAsync(new CreateCraftingNotionInventoryItemDto
        {
            BrandName = "Other",
            TypeName = "Marker",
            Quantity = 1
        }, otherUser.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.AddInventoryLinkAsync(project.Id, new CreateCraftingProjectInventoryLinkDto
            {
                InventoryItemType = "notion",
                InventoryItemId = otherNotion.Id,
                QuantityPlanned = 1
            }, owner.Id));

        Assert.Equal("The selected inventory item is not available for this project.", rejected.Message);
    }

    [Fact]
    public async Task RemoveInventoryLinkAsync_removes_owned_project_link()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var projectService = new CraftingProjectService(context);
        var inventoryService = new CraftingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateCraftingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var yarn = await inventoryService.CreateOrMergeYarnAsync(new CreateCraftingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Blue",
            Skeins = 2
        }, user.Id);
        var linked = await projectService.AddInventoryLinkAsync(project.Id, new CreateCraftingProjectInventoryLinkDto
        {
            InventoryItemType = "yarn",
            InventoryItemId = yarn.Id,
            QuantityPlanned = 1
        }, user.Id);

        Assert.NotNull(linked);

        var removed = await projectService.RemoveInventoryLinkAsync(project.Id, linked.InventoryLinks[0].Id, user.Id);
        var reloaded = await projectService.GetByIdAsync(project.Id, user.Id);

        Assert.True(removed);
        Assert.NotNull(reloaded);
        Assert.Empty(reloaded.InventoryLinks);
    }
}
