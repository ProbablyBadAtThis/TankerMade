using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Kits;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Server.Services.Crafting;
using Xunit;

namespace TankerMade.Tests;

public class CraftingKitServiceTests
{
    [Fact]
    public async Task CreateAsync_persists_user_scoped_kit()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);

        var created = await service.CreateAsync(new CreateCraftingKitDto
        {
            Name = "Winter Gift Set",
            Description = "Grouped pieces for a gift.",
            Type = "crochet",
            Difficulty = 2,
            Progress = 10
        }, owner.Id);

        var ownerList = await service.GetAllAsync(owner.Id);
        var otherList = await service.GetAllAsync(otherUser.Id);

        Assert.Equal("Winter Gift Set", created.Name);
        Assert.Equal("winter-gift-set", created.Slug);
        Assert.Equal(10, created.Progress);
        Assert.Single(ownerList);
        Assert.Empty(otherList);
    }

    [Fact]
    public async Task AddPieceAsync_links_only_owned_patterns()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var patternService = new CraftingPatternService(context);
        var kitService = new CraftingKitService(context);
        var kit = await kitService.CreateAsync(new CreateCraftingKitDto { Name = "Sweater Kit" }, owner.Id);
        var ownedPattern = await patternService.CreateAsync(new CreateCraftingPatternDto { Name = "Sleeve" }, owner.Id);
        var otherPattern = await patternService.CreateAsync(new CreateCraftingPatternDto { Name = "Other Sleeve" }, otherUser.Id);

        var piece = await kitService.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto
        {
            Name = "Left sleeve",
            PatternId = ownedPattern.Id,
            Notes = "Use smaller hook."
        }, owner.Id);
        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            kitService.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto
            {
                Name = "Other user's piece",
                PatternId = otherPattern.Id
            }, owner.Id));

        Assert.NotNull(piece);
        Assert.Equal(ownedPattern.Id, piece.PatternId);
        Assert.Equal("Sleeve", piece.PatternName);
        Assert.Equal("The selected pattern is not available for this kit.", rejected.Message);
    }

    [Fact]
    public async Task ReorderPiecesAsync_requires_same_piece_set()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);
        var kit = await service.CreateAsync(new CreateCraftingKitDto { Name = "Blanket Kit" }, user.Id);
        var first = await service.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto { Name = "Panel A" }, user.Id);
        var second = await service.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto { Name = "Panel B" }, user.Id);

        Assert.NotNull(first);
        Assert.NotNull(second);

        var rejected = await service.ReorderPiecesAsync(kit.Id, new ReorderCraftingKitItemsDto
        {
            OrderedIds = [first.Id]
        }, user.Id);
        var reordered = await service.ReorderPiecesAsync(kit.Id, new ReorderCraftingKitItemsDto
        {
            OrderedIds = [second.Id, first.Id]
        }, user.Id);
        var reloaded = await service.GetByIdAsync(kit.Id, user.Id);

        Assert.False(rejected);
        Assert.True(reordered);
        Assert.NotNull(reloaded);
        Assert.Equal(second.Id, reloaded.Pieces[0].Id);
        Assert.Equal(first.Id, reloaded.Pieces[1].Id);
    }

    [Fact]
    public async Task Supplies_are_text_based_and_ordered_inside_kit()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);
        var kit = await service.CreateAsync(new CreateCraftingKitDto { Name = "Market Set" }, user.Id);
        var yarn = await service.AddSupplyAsync(kit.Id, new CreateCraftingKitSupplyDto
        {
            SupplyType = "yarn",
            Name = "Two skeins of worsted",
            Quantity = 2
        }, user.Id);
        var buttons = await service.AddSupplyAsync(kit.Id, new CreateCraftingKitSupplyDto
        {
            SupplyType = "notion",
            Name = "Buttons",
            Quantity = 6,
            Notes = "Wood preferred"
        }, user.Id);

        Assert.NotNull(yarn);
        Assert.NotNull(buttons);

        await service.ReorderSuppliesAsync(kit.Id, new ReorderCraftingKitItemsDto
        {
            OrderedIds = [buttons.Id, yarn.Id]
        }, user.Id);
        var reloaded = await service.GetByIdAsync(kit.Id, user.Id);

        Assert.NotNull(reloaded);
        Assert.Equal("notion", reloaded.Supplies[0].SupplyType);
        Assert.Equal("Buttons", reloaded.Supplies[0].Name);
        Assert.Equal(6, reloaded.Supplies[0].Quantity);
        Assert.Equal("yarn", reloaded.Supplies[1].SupplyType);
    }

    [Fact]
    public async Task CreateProjectForPieceAsync_creates_sub_project_with_piece_pattern()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var patternService = new CraftingPatternService(context);
        var kitService = new CraftingKitService(context);
        var pattern = await patternService.CreateAsync(new CreateCraftingPatternDto { Name = "Hat Pattern" }, user.Id);
        var kit = await kitService.CreateAsync(new CreateCraftingKitDto
        {
            Name = "Winter Set",
            Difficulty = 3
        }, user.Id);
        var piece = await kitService.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto
        {
            Name = "Hat",
            PatternId = pattern.Id,
            Notes = "Make this first."
        }, user.Id);

        Assert.NotNull(piece);

        var project = await kitService.CreateProjectForPieceAsync(
            kit.Id,
            piece.Id,
            new CreateCraftingKitProjectDto(),
            user.Id);

        Assert.NotNull(project);
        Assert.Equal("Winter Set - Hat", project.Name);
        Assert.Equal("Make this first.", project.Description);
        Assert.Equal(pattern.Id, project.PatternId);
        Assert.Equal(kit.Id, project.KitId);
        Assert.Equal("Winter Set", project.KitName);
        Assert.Equal(piece.Id, project.KitPieceId);
        Assert.Equal("Hat", project.KitPieceName);
        Assert.Equal(3, project.Difficulty);
    }

    [Fact]
    public async Task CreateProjectForPieceAsync_allows_only_one_project_per_piece()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);
        var kit = await service.CreateAsync(new CreateCraftingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto { Name = "Scarf" }, user.Id);

        Assert.NotNull(piece);

        await service.CreateProjectForPieceAsync(kit.Id, piece.Id, new CreateCraftingKitProjectDto(), user.Id);
        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateProjectForPieceAsync(kit.Id, piece.Id, new CreateCraftingKitProjectDto(), user.Id));

        Assert.Equal("This kit piece already has a project.", rejected.Message);
    }

    [Fact]
    public async Task DeletePieceAsync_clears_project_kit_backlink()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);
        var projectService = new CraftingProjectService(context);
        var kit = await service.CreateAsync(new CreateCraftingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto { Name = "Hat" }, user.Id);

        Assert.NotNull(piece);

        var project = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, new CreateCraftingKitProjectDto(), user.Id);
        var deleted = await service.DeletePieceAsync(kit.Id, piece.Id, user.Id);
        var reloadedProject = await projectService.GetByIdAsync(project!.Id, user.Id);

        Assert.True(deleted);
        Assert.NotNull(reloadedProject);
        Assert.Null(reloadedProject.KitId);
        Assert.Null(reloadedProject.KitPieceId);
    }

    [Fact]
    public async Task DeleteAsync_clears_project_kit_backlinks()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingKitService(context);
        var projectService = new CraftingProjectService(context);
        var kit = await service.CreateAsync(new CreateCraftingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateCraftingKitPieceDto { Name = "Hat" }, user.Id);

        Assert.NotNull(piece);

        var project = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, new CreateCraftingKitProjectDto(), user.Id);
        var deleted = await service.DeleteAsync(kit.Id, user.Id);
        var reloadedProject = await projectService.GetByIdAsync(project!.Id, user.Id);

        Assert.True(deleted);
        Assert.NotNull(reloadedProject);
        Assert.Null(reloadedProject.KitId);
        Assert.Null(reloadedProject.KitPieceId);
    }
}
