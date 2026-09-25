using TankerMade.Core.Entities;
using TankerMade.Modules.Knitting.DTOs.Inventory;
using TankerMade.Modules.Knitting.DTOs.Kits;
using TankerMade.Modules.Knitting.DTOs.Patterns;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Server.Services.Knitting;
using Xunit;

namespace TankerMade.Tests;

public class KnittingDomainServiceTests
{
    // --- Projects ---

    [Fact]
    public async Task CreateAsync_persists_project_in_knitting_module_table()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingProjectService(context);

        var created = await service.CreateAsync(new CreateKnittingProjectDto
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

        var patternService = new KnittingPatternService(context);
        var projectService = new KnittingProjectService(context);
        var ownedPattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Owned Pattern"
        }, owner.Id);
        var otherPattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Other Pattern"
        }, otherUser.Id);

        var created = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Linked Project",
            PatternId = ownedPattern.Id
        }, owner.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.CreateAsync(new CreateKnittingProjectDto
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

        var patternService = new KnittingPatternService(context);
        var projectService = new KnittingProjectService(context);
        var pattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Linked Pattern"
        }, user.Id);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Linked Project",
            PatternId = pattern.Id
        }, user.Id);

        var updated = await projectService.UpdateAsync(new UpdateKnittingProjectDto
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

        var service = new KnittingProjectService(context);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateKnittingProjectDto
            {
                Name = "Too Far",
                Progress = 101
            }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateKnittingProjectDto
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

        var patternService = new KnittingPatternService(context);
        var projectService = new KnittingProjectService(context);
        var pattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Tracked Pattern"
        }, user.Id);
        var piece = await patternService.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Main"
        }, user.Id);

        Assert.NotNull(piece);

        var firstStep = await patternService.AddStepAsync(pattern.Id, piece.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 1,
            RangeEnd = 1,
            Label = "Start",
            Instructions = "Start here."
        }, user.Id);

        Assert.NotNull(firstStep);

        await patternService.AddStepAsync(pattern.Id, piece.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 2,
            RangeEnd = 2,
            Label = "Continue",
            Instructions = "Keep going."
        }, user.Id);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Tracked Project",
            PatternId = pattern.Id
        }, user.Id);

        var checkedProject = await projectService.SetStepProgressAsync(
            project.Id,
            firstStep.Id,
            new UpdateKnittingProjectStepProgressDto { IsComplete = true },
            user.Id);
        var uncheckedProject = await projectService.SetStepProgressAsync(
            project.Id,
            firstStep.Id,
            new UpdateKnittingProjectStepProgressDto { IsComplete = false },
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

        var patternService = new KnittingPatternService(context);
        var projectService = new KnittingProjectService(context);
        var linkedPattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Linked Pattern"
        }, user.Id);
        var linkedPiece = await patternService.AddPieceAsync(linkedPattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Linked Piece"
        }, user.Id);
        var otherPattern = await patternService.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Other Pattern"
        }, user.Id);
        var otherPiece = await patternService.AddPieceAsync(otherPattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Other Piece"
        }, user.Id);

        Assert.NotNull(linkedPiece);
        Assert.NotNull(otherPiece);

        await patternService.AddStepAsync(linkedPattern.Id, linkedPiece.Id, new CreateKnittingPatternStepDto
        {
            Label = "Allowed",
            Instructions = "This one belongs."
        }, user.Id);
        var otherStep = await patternService.AddStepAsync(otherPattern.Id, otherPiece.Id, new CreateKnittingPatternStepDto
        {
            Label = "Rejected",
            Instructions = "This one does not."
        }, user.Id);

        Assert.NotNull(otherStep);

        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Linked Project",
            PatternId = linkedPattern.Id
        }, user.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.SetStepProgressAsync(
                project.Id,
                otherStep.Id,
                new UpdateKnittingProjectStepProgressDto { IsComplete = true },
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

        var projectService = new KnittingProjectService(context);
        var inventoryService = new KnittingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var yarn = await inventoryService.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Blue",
            Skeins = 2
        }, user.Id);

        var linked = await projectService.AddInventoryLinkAsync(project.Id, new CreateKnittingProjectInventoryLinkDto
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

        var projectService = new KnittingProjectService(context);
        var inventoryService = new KnittingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var tool = await inventoryService.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Acme",
            TypeName = "Needle",
            Quantity = 1
        }, user.Id);

        await projectService.AddInventoryLinkAsync(project.Id, new CreateKnittingProjectInventoryLinkDto
        {
            InventoryItemType = "tool",
            InventoryItemId = tool.Id,
            QuantityPlanned = 1,
            Notes = "Original"
        }, user.Id);
        var updated = await projectService.AddInventoryLinkAsync(project.Id, new CreateKnittingProjectInventoryLinkDto
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

        var projectService = new KnittingProjectService(context);
        var inventoryService = new KnittingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Supply Project"
        }, owner.Id);
        var otherNotion = await inventoryService.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Other",
            TypeName = "Marker",
            Quantity = 1
        }, otherUser.Id);

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            projectService.AddInventoryLinkAsync(project.Id, new CreateKnittingProjectInventoryLinkDto
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

        var projectService = new KnittingProjectService(context);
        var inventoryService = new KnittingInventoryService(context);
        var project = await projectService.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Supply Project"
        }, user.Id);
        var yarn = await inventoryService.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Blue",
            Skeins = 2
        }, user.Id);
        var linked = await projectService.AddInventoryLinkAsync(project.Id, new CreateKnittingProjectInventoryLinkDto
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

    // --- Patterns ---

    [Fact]
    public async Task UpdateAsync_preserves_existing_values_when_fields_are_omitted_or_blank()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingPatternService(context);
        var created = await service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Original Pattern",
            Type = "Sweater",
            Form = "Wearable",
            Difficulty = "Intermediate"
        }, user.Id);

        var updated = await service.UpdateAsync(new UpdateKnittingPatternDto
        {
            Id = created.Id,
            Name = "Renamed Pattern",
            Type = "",
            Form = "   "
        }, user.Id);

        Assert.NotNull(updated);
        Assert.Equal("Renamed Pattern", updated.Name);
        Assert.Equal("renamed-pattern", updated.Slug);
        Assert.Equal("Sweater", updated.Type);
        Assert.Equal("Wearable", updated.Form);
        Assert.Equal("Intermediate", updated.Difficulty);
    }

    [Fact]
    public async Task AddPieceAndStepAsync_returns_pattern_detail_with_ordered_template_content()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingPatternService(context);
        var pattern = await service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Reference Pattern"
        }, user.Id);

        var body = await service.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Body"
        }, user.Id);

        var sleeve = await service.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Sleeve"
        }, user.Id);

        Assert.NotNull(body);
        Assert.NotNull(sleeve);
        Assert.Equal(1, body.SortOrder);
        Assert.Equal(2, sleeve.SortOrder);

        var step = await service.AddStepAsync(pattern.Id, body.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 5,
            RangeEnd = 7,
            Label = "Repeat",
            Instructions = "Work the established repeat."
        }, user.Id);

        Assert.NotNull(step);
        Assert.Equal("5-7", step.DisplayRange);

        var detail = await service.GetByIdAsync(pattern.Id, user.Id);

        Assert.NotNull(detail);
        Assert.Equal(2, detail.PieceCount);
        Assert.Equal(1, detail.StepCount);
        Assert.Equal(["Body", "Sleeve"], detail.Pieces.Select(p => p.Name).ToArray());
        Assert.Equal("Work the established repeat.", detail.Pieces[0].Steps[0].Instructions);
        // Knitting has no Progress readiness DTO; incomplete readiness is reflected by step coverage.
        Assert.True(detail.PieceCount > detail.StepCount);
    }

    [Fact]
    public async Task ReorderPiecesAndStepsAsync_requires_owned_complete_item_sets()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new KnittingPatternService(context);
        var pattern = await service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Owned Pattern"
        }, owner.Id);

        var first = await service.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto { Name = "First" }, owner.Id);
        var second = await service.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto { Name = "Second" }, owner.Id);

        Assert.NotNull(first);
        Assert.NotNull(second);

        var stepOne = await service.AddStepAsync(pattern.Id, first.Id, new CreateKnittingPatternStepDto
        {
            Label = "A",
            Instructions = "First instruction"
        }, owner.Id);

        var stepTwo = await service.AddStepAsync(pattern.Id, first.Id, new CreateKnittingPatternStepDto
        {
            Label = "B",
            Instructions = "Second instruction"
        }, owner.Id);

        Assert.NotNull(stepOne);
        Assert.NotNull(stepTwo);

        var otherUserCannotReorder = await service.ReorderPiecesAsync(pattern.Id, new ReorderKnittingPatternItemsDto
        {
            OrderedIds = [second.Id, first.Id]
        }, otherUser.Id);

        var incompleteSetCannotReorder = await service.ReorderStepsAsync(pattern.Id, first.Id, new ReorderKnittingPatternItemsDto
        {
            OrderedIds = [stepTwo.Id]
        }, owner.Id);

        var reorderedPieces = await service.ReorderPiecesAsync(pattern.Id, new ReorderKnittingPatternItemsDto
        {
            OrderedIds = [second.Id, first.Id]
        }, owner.Id);

        var reorderedSteps = await service.ReorderStepsAsync(pattern.Id, first.Id, new ReorderKnittingPatternItemsDto
        {
            OrderedIds = [stepTwo.Id, stepOne.Id]
        }, owner.Id);

        var detail = await service.GetByIdAsync(pattern.Id, owner.Id);

        Assert.False(otherUserCannotReorder);
        Assert.False(incompleteSetCannotReorder);
        Assert.True(reorderedPieces);
        Assert.True(reorderedSteps);
        Assert.NotNull(detail);
        Assert.Equal(["Second", "First"], detail.Pieces.Select(p => p.Name).ToArray());
        Assert.Equal(["B", "A"], detail.Pieces[1].Steps.Select(s => s.Label).ToArray());
    }

    [Fact]
    public async Task GetByIdAsync_returns_empty_counts_for_empty_pattern()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingPatternService(context);
        var pattern = await service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Empty Pattern"
        }, user.Id);

        var detail = await service.GetByIdAsync(pattern.Id, user.Id);

        Assert.NotNull(detail);
        Assert.Equal(0, detail.PieceCount);
        Assert.Equal(0, detail.StepCount);
        Assert.Empty(detail.Pieces);
    }

    [Fact]
    public async Task GetByIdAsync_marks_pattern_ready_when_it_has_piece_and_step_content()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingPatternService(context);
        var pattern = await service.CreateAsync(new CreateKnittingPatternDto
        {
            Name = "Ready Pattern"
        }, user.Id);
        var piece = await service.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto
        {
            Name = "Main"
        }, user.Id);

        Assert.NotNull(piece);

        await service.AddStepAsync(pattern.Id, piece.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 1,
            RangeEnd = 2,
            Label = "Build",
            Instructions = "Make the thing."
        }, user.Id);

        var detail = await service.GetByIdAsync(pattern.Id, user.Id);

        Assert.NotNull(detail);
        Assert.True(detail.PieceCount > 0);
        Assert.True(detail.StepCount > 0);
        Assert.All(detail.Pieces, p => Assert.NotEmpty(p.Steps));
    }

    // --- Kits ---

    [Fact]
    public async Task CreateAsync_persists_user_scoped_kit()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new KnittingKitService(context);

        var created = await service.CreateAsync(new CreateKnittingKitDto
        {
            Name = "Winter Gift Set",
            Description = "Grouped pieces for a gift.",
            Type = "sweater"
        }, owner.Id);

        var ownerList = await service.GetAllAsync(owner.Id);
        var otherList = await service.GetAllAsync(otherUser.Id);

        Assert.Equal("Winter Gift Set", created.Name);
        Assert.Equal("Grouped pieces for a gift.", created.Description);
        Assert.Equal("sweater", created.Type);
        Assert.Single(ownerList);
        Assert.Empty(otherList);
    }

    [Fact]
    public async Task Supplies_are_text_based_and_ordered_inside_kit()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingKitService(context);
        var kit = await service.CreateAsync(new CreateKnittingKitDto { Name = "Market Set" }, user.Id);
        var yarn = await service.AddSupplyAsync(kit.Id, new CreateKnittingKitSupplyDto
        {
            SupplyType = "yarn",
            Name = "Two skeins of worsted",
            Quantity = 2
        }, user.Id);
        var buttons = await service.AddSupplyAsync(kit.Id, new CreateKnittingKitSupplyDto
        {
            SupplyType = "notion",
            Name = "Buttons",
            Quantity = 6
        }, user.Id);

        Assert.NotNull(yarn);
        Assert.NotNull(buttons);
        Assert.Equal(1, yarn.SortOrder);
        Assert.Equal(2, buttons.SortOrder);

        var reloaded = await service.GetByIdAsync(kit.Id, user.Id);

        Assert.NotNull(reloaded);
        Assert.Equal(2, reloaded.Supplies.Count);
        Assert.Equal("yarn", reloaded.Supplies[0].SupplyType);
        Assert.Equal("Two skeins of worsted", reloaded.Supplies[0].Name);
        Assert.Equal(2, reloaded.Supplies[0].Quantity);
        Assert.Equal("notion", reloaded.Supplies[1].SupplyType);
        Assert.Equal("Buttons", reloaded.Supplies[1].Name);
        Assert.Equal(6, reloaded.Supplies[1].Quantity);
    }

    [Fact]
    public async Task CreateProjectForPieceAsync_creates_sub_project_from_piece()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var kitService = new KnittingKitService(context);
        var kit = await kitService.CreateAsync(new CreateKnittingKitDto
        {
            Name = "Winter Set"
        }, user.Id);
        var piece = await kitService.AddPieceAsync(kit.Id, new CreateKnittingKitPieceDto
        {
            Name = "Hat",
            Notes = "Make this first."
        }, user.Id);

        Assert.NotNull(piece);

        var project = await kitService.CreateProjectForPieceAsync(kit.Id, piece.Id, user.Id);
        var reloadedKit = await kitService.GetByIdAsync(kit.Id, user.Id);

        Assert.NotNull(project);
        Assert.Equal("Hat", project.Name);
        Assert.Equal("Make this first.", project.Description);
        Assert.NotNull(reloadedKit);
        Assert.Equal(project.Id, reloadedKit.Pieces[0].ProjectId);
    }

    [Fact]
    public async Task CreateProjectForPieceAsync_allows_only_one_project_per_piece()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingKitService(context);
        var kit = await service.CreateAsync(new CreateKnittingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateKnittingKitPieceDto { Name = "Scarf" }, user.Id);

        Assert.NotNull(piece);

        var first = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, user.Id);
        var second = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, user.Id);

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task DeletePieceAsync_clears_project_kit_backlink()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingKitService(context);
        var projectService = new KnittingProjectService(context);
        var kit = await service.CreateAsync(new CreateKnittingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateKnittingKitPieceDto { Name = "Hat" }, user.Id);

        Assert.NotNull(piece);

        var project = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, user.Id);
        var deleted = await service.DeletePieceAsync(kit.Id, piece.Id, user.Id);
        var reloadedProject = await projectService.GetByIdAsync(project!.Id, user.Id);
        var reloadedKit = await service.GetByIdAsync(kit.Id, user.Id);

        Assert.True(deleted);
        Assert.NotNull(reloadedProject);
        Assert.NotNull(reloadedKit);
        Assert.Empty(reloadedKit.Pieces);
        Assert.DoesNotContain(reloadedKit.Pieces, p => p.ProjectId == project.Id);
    }

    [Fact]
    public async Task DeleteAsync_clears_project_kit_backlinks()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingKitService(context);
        var projectService = new KnittingProjectService(context);
        var kit = await service.CreateAsync(new CreateKnittingKitDto { Name = "Gift Kit" }, user.Id);
        var piece = await service.AddPieceAsync(kit.Id, new CreateKnittingKitPieceDto { Name = "Hat" }, user.Id);

        Assert.NotNull(piece);

        var project = await service.CreateProjectForPieceAsync(kit.Id, piece.Id, user.Id);
        var deleted = await service.DeleteAsync(kit.Id, user.Id);
        var reloadedProject = await projectService.GetByIdAsync(project!.Id, user.Id);
        var reloadedKit = await service.GetByIdAsync(kit.Id, user.Id);

        Assert.True(deleted);
        Assert.NotNull(reloadedProject);
        Assert.Null(reloadedKit);
    }

    // --- Inventory ---

    [Fact]
    public async Task CreateOrMergeYarnAsync_merges_by_user_brand_and_color()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var first = await service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Storm Blue",
            MainColor = "Blue",
            WeightName = "Worsted",
            FiberContent = "100% wool",
            FiberTag = "Natural",
            Skeins = 2,
            EstimatedLength = 440,
            LotNumber = "A1",
            SourceName = "Local Shop",
            Price = 12.50m
        }, user.Id);

        var merged = await service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = " acme yarn ",
            ColorName = "storm blue",
            Skeins = 3,
            EstimatedLength = 660,
            LotNumber = "A1",
            SourceName = "Local Shop",
            Price = 9.99m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(5, merged.TotalSkeins);
        Assert.Equal(1100, merged.EstimatedRemainingLength);
        Assert.Equal(12.50m, merged.RegularPrice);
        Assert.Single(merged.Lots);
        Assert.Equal(5, merged.Lots[0].Skeins);
        Assert.Equal(1100, merged.Lots[0].RemainingLength);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 9.99m);
    }

    [Fact]
    public async Task CreateOrMergeYarnAsync_keeps_users_inventory_separate()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);
        var request = new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Shared Brand",
            ColorName = "Same Color",
            Skeins = 1
        };

        var ownerYarn = await service.CreateOrMergeYarnAsync(request, owner.Id);
        var otherYarn = await service.CreateOrMergeYarnAsync(request, otherUser.Id);
        var ownerList = await service.GetYarnsAsync(owner.Id);
        var otherList = await service.GetYarnsAsync(otherUser.Id);

        Assert.NotEqual(ownerYarn.Id, otherYarn.Id);
        Assert.Single(ownerList);
        Assert.Single(otherList);
        Assert.Equal(ownerYarn.Id, ownerList[0].Id);
        Assert.Equal(otherYarn.Id, otherList[0].Id);
        Assert.Null(await service.GetYarnByIdAsync(ownerYarn.Id, otherUser.Id));
    }

    [Fact]
    public async Task CreateOrMergeYarnAsync_rejects_missing_identity_or_quantity()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "",
            ColorName = "Blue",
            Skeins = 1
        }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Acme",
            ColorName = "Blue",
            Skeins = 0
        }, user.Id));
    }

    [Fact]
    public async Task CreateOrMergeToolAsync_merges_by_user_brand_and_type()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var first = await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Acme Tools",
            TypeName = "Needle",
            Size = "5 mm",
            Description = "Aluminum needle",
            Quantity = 1,
            SourceName = "Local Shop",
            Price = 7.50m
        }, user.Id);

        var merged = await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = " acme tools ",
            TypeName = "needle",
            Quantity = 2,
            SourceName = "Local Shop",
            Price = 4.99m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(3, merged.Quantity);
        Assert.Equal("5 mm", merged.Size);
        Assert.Equal(7.50m, merged.RegularPrice);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 4.99m);
    }

    [Fact]
    public async Task CreateOrMergeNotionAsync_merges_by_user_brand_and_type()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var first = await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Acme Notions",
            TypeName = "Button",
            Size = "12 mm",
            ColorName = "Black",
            Description = "Matte buttons",
            Quantity = 6,
            SourceName = "Craft Store",
            Price = 3.25m
        }, user.Id);

        var merged = await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "acme notions",
            TypeName = " button ",
            Quantity = 4,
            SourceName = "Craft Store",
            Price = 2.00m,
            IsSalePrice = true
        }, user.Id);

        Assert.Equal(first.Id, merged.Id);
        Assert.Equal(10, merged.Quantity);
        Assert.Equal("12 mm", merged.Size);
        Assert.Equal("Black", merged.ColorName);
        Assert.Equal(3.25m, merged.RegularPrice);
        Assert.Equal(2, merged.Purchases.Count);
        Assert.Contains(merged.Purchases, p => p.IsSalePrice && p.Price == 2.00m);
    }

    [Fact]
    public async Task ToolAndNotion_inventory_is_user_scoped()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var owner = new User(Guid.NewGuid(), "owner", "owner@example.test", "hash");
        var otherUser = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(owner, otherUser);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var ownerTool = await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Needle",
            Quantity = 1
        }, owner.Id);
        var otherTool = await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Needle",
            Quantity = 1
        }, otherUser.Id);

        var ownerNotion = await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Marker",
            Quantity = 1
        }, owner.Id);
        var otherNotion = await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Shared Brand",
            TypeName = "Marker",
            Quantity = 1
        }, otherUser.Id);

        Assert.NotEqual(ownerTool.Id, otherTool.Id);
        Assert.NotEqual(ownerNotion.Id, otherNotion.Id);
        Assert.Null(await service.GetToolByIdAsync(ownerTool.Id, otherUser.Id));
        Assert.Null(await service.GetNotionByIdAsync(ownerNotion.Id, otherUser.Id));
    }

    [Fact]
    public async Task ToolAndNotion_inventory_rejects_missing_identity_or_quantity()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "",
            TypeName = "Needle",
            Quantity = 1
        }, user.Id));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Acme",
            TypeName = "Marker",
            Quantity = 0
        }, user.Id));
    }

    [Fact]
    public async Task GetYarnsAsync_applies_inventory_filters()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var blue = await service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Acme Yarn",
            ColorName = "Storm Blue",
            MainColor = "Blue",
            WeightName = "Worsted",
            FiberContent = "100% wool",
            FiberTag = "Natural",
            Skeins = 2,
            SourceName = "Local Shop"
        }, user.Id);

        await service.CreateOrMergeYarnAsync(new CreateKnittingYarnInventoryItemDto
        {
            BrandName = "Bright Yarn",
            ColorName = "Sunset",
            MainColor = "Orange",
            WeightName = "DK",
            FiberTag = "Synthetic",
            Skeins = 1,
            SourceName = "Online"
        }, user.Id);

        var byBrand = await service.GetYarnsAsync(user.Id, new KnittingYarnInventoryFilterDto
        {
            BrandName = "acme"
        });
        var bySource = await service.GetYarnsAsync(user.Id, new KnittingYarnInventoryFilterDto
        {
            SourceName = "Local"
        });
        var bySearch = await service.GetYarnsAsync(user.Id, new KnittingYarnInventoryFilterDto
        {
            Search = "wool"
        });

        Assert.Single(byBrand);
        Assert.Equal(blue.Id, byBrand[0].Id);
        Assert.Single(bySource);
        Assert.Equal(blue.Id, bySource[0].Id);
        Assert.Single(bySearch);
        Assert.Equal(blue.Id, bySearch[0].Id);
    }

    [Fact]
    public async Task GetToolsAndNotionsAsync_apply_inventory_filters()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new KnittingInventoryService(context);

        var needle = await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Acme Tools",
            TypeName = "Needle",
            Size = "5 mm",
            Description = "Aluminum needle",
            Quantity = 1,
            SourceName = "Local Shop"
        }, user.Id);
        await service.CreateOrMergeToolAsync(new CreateKnittingToolInventoryItemDto
        {
            BrandName = "Other Tools",
            TypeName = "Cable Needle",
            Size = "7 mm",
            Quantity = 1,
            SourceName = "Online"
        }, user.Id);

        var button = await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Acme Notions",
            TypeName = "Button",
            Size = "12 mm",
            ColorName = "Black",
            Description = "Matte buttons",
            Quantity = 6,
            SourceName = "Craft Store"
        }, user.Id);
        await service.CreateOrMergeNotionAsync(new CreateKnittingNotionInventoryItemDto
        {
            BrandName = "Other Notions",
            TypeName = "Marker",
            ColorName = "Green",
            Quantity = 2,
            SourceName = "Online"
        }, user.Id);

        var toolFilter = await service.GetToolsAsync(user.Id, new KnittingToolInventoryFilterDto
        {
            TypeName = "needle",
            SourceName = "Local"
        });
        var notionFilter = await service.GetNotionsAsync(user.Id, new KnittingNotionInventoryFilterDto
        {
            ColorName = "Black",
            Search = "Matte"
        });

        Assert.Single(toolFilter);
        Assert.Equal(needle.Id, toolFilter[0].Id);
        Assert.Single(notionFilter);
        Assert.Equal(button.Id, notionFilter[0].Id);
    }

    [Fact]
    public async Task GetReferenceItemsAsync_returns_seeded_module_reference_values_by_category()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new KnittingInventoryService(context);

        var yarnWeights = await service.GetReferenceItemsAsync("yarn-weight");
        var fiberTags = await service.GetReferenceItemsAsync("fiber-tag");
        var unknown = await service.GetReferenceItemsAsync("missing-category");

        Assert.Contains(yarnWeights, item => item.Name == "Worsted" && item.Slug == "worsted");
        Assert.Equal(yarnWeights.OrderBy(item => item.SortOrder).Select(item => item.Id), yarnWeights.Select(item => item.Id));
        Assert.Contains(fiberTags, item => item.Name == "Natural");
        Assert.Empty(unknown);
    }

    [Fact]
    public async Task GetReferenceItemsAsync_returns_core_reference_categories_for_module_extension_points()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new KnittingInventoryService(context);

        var themes = await service.GetReferenceItemsAsync("themes");
        var sources = await service.GetReferenceItemsAsync("source");

        Assert.NotEmpty(themes);
        Assert.Contains(themes, item => item.Category == "theme" && item.Name == "Animals");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, item => item.Category == "source" && item.Name == "Website");
    }

    [Fact]
    public async Task GetReferenceItemsAsync_does_not_treat_other_module_categories_as_knitting_owned()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new KnittingInventoryService(context);

        var printingCategory = await service.GetReferenceItemsAsync("material-type");

        Assert.Empty(printingCategory);
    }

    [Fact]
    public async Task CreateReferenceItemAsync_creates_and_reuses_module_owned_values()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new KnittingInventoryService(context);

        var created = await service.CreateReferenceItemAsync("tool-type", new CreateKnittingInventoryReferenceItemDto
        {
            Name = "Cable Needle"
        });
        var duplicate = await service.CreateReferenceItemAsync("tool-type", new CreateKnittingInventoryReferenceItemDto
        {
            Name = " cable needle "
        });

        Assert.Equal(created.Id, duplicate.Id);
        var items = await service.GetReferenceItemsAsync("tool-type");
        Assert.Contains(items, item => item.Name == "Cable Needle");
    }

    [Fact]
    public async Task CreateReferenceItemAsync_rejects_non_module_owned_categories()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();

        var service = new KnittingInventoryService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateReferenceItemAsync("theme", new CreateKnittingInventoryReferenceItemDto
        {
            Name = "Ocean"
        }));
    }
}
