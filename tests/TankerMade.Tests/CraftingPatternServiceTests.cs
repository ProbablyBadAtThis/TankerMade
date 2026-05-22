using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Server.Services.Crafting;
using Xunit;

namespace TankerMade.Tests;

public class CraftingPatternServiceTests
{
    [Fact]
    public async Task UpdateAsync_preserves_existing_values_when_fields_are_omitted_or_blank()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingPatternService(context);
        var created = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Original Pattern",
            Type = "Knitting",
            Form = "Wearable",
            Difficulty = "Intermediate"
        }, user.Id);

        var updated = await service.UpdateAsync(new UpdateCraftingPatternDto
        {
            Id = created.Id,
            Name = "Renamed Pattern",
            Type = "",
            Form = "   "
        }, user.Id);

        Assert.NotNull(updated);
        Assert.Equal("Renamed Pattern", updated.Name);
        Assert.Equal("renamed-pattern", updated.Slug);
        Assert.Equal("Knitting", updated.Type);
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

        var service = new CraftingPatternService(context);
        var pattern = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Reference Pattern"
        }, user.Id);

        var body = await service.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Body"
        }, user.Id);

        var sleeve = await service.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Sleeve"
        }, user.Id);

        Assert.NotNull(body);
        Assert.NotNull(sleeve);
        Assert.Equal(1, body.SortOrder);
        Assert.Equal(2, sleeve.SortOrder);

        var step = await service.AddStepAsync(pattern.Id, body.Id, new CreateCraftingPatternStepDto
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
        Assert.False(detail.Progress.IsReadyForProject);
        Assert.Contains("1 piece(s) do not have steps yet.", detail.Progress.ValidationMessages);
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

        var service = new CraftingPatternService(context);
        var pattern = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Owned Pattern"
        }, owner.Id);

        var first = await service.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto { Name = "First" }, owner.Id);
        var second = await service.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto { Name = "Second" }, owner.Id);

        Assert.NotNull(first);
        Assert.NotNull(second);

        var stepOne = await service.AddStepAsync(pattern.Id, first.Id, new CreateCraftingPatternStepDto
        {
            Label = "A",
            Instructions = "First instruction"
        }, owner.Id);

        var stepTwo = await service.AddStepAsync(pattern.Id, first.Id, new CreateCraftingPatternStepDto
        {
            Label = "B",
            Instructions = "Second instruction"
        }, owner.Id);

        Assert.NotNull(stepOne);
        Assert.NotNull(stepTwo);

        var otherUserCannotReorder = await service.ReorderPiecesAsync(pattern.Id, new ReorderCraftingPatternItemsDto
        {
            OrderedIds = [second.Id, first.Id]
        }, otherUser.Id);

        var incompleteSetCannotReorder = await service.ReorderStepsAsync(pattern.Id, first.Id, new ReorderCraftingPatternItemsDto
        {
            OrderedIds = [stepTwo.Id]
        }, owner.Id);

        var reorderedPieces = await service.ReorderPiecesAsync(pattern.Id, new ReorderCraftingPatternItemsDto
        {
            OrderedIds = [second.Id, first.Id]
        }, owner.Id);

        var reorderedSteps = await service.ReorderStepsAsync(pattern.Id, first.Id, new ReorderCraftingPatternItemsDto
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
    public async Task GetByIdAsync_returns_progress_validation_for_empty_pattern()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingPatternService(context);
        var pattern = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Empty Pattern"
        }, user.Id);

        var detail = await service.GetByIdAsync(pattern.Id, user.Id);

        Assert.NotNull(detail);
        Assert.False(detail.Progress.HasPieces);
        Assert.False(detail.Progress.HasSteps);
        Assert.False(detail.Progress.IsReadyForProject);
        Assert.Equal(2, detail.Progress.ValidationMessages.Count);
    }

    [Fact]
    public async Task GetByIdAsync_marks_pattern_ready_when_it_has_piece_and_step_content()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new CraftingPatternService(context);
        var pattern = await service.CreateAsync(new CreateCraftingPatternDto
        {
            Name = "Ready Pattern"
        }, user.Id);
        var piece = await service.AddPieceAsync(pattern.Id, new CreateCraftingPatternPieceDto
        {
            Name = "Main"
        }, user.Id);

        Assert.NotNull(piece);

        await service.AddStepAsync(pattern.Id, piece.Id, new CreateCraftingPatternStepDto
        {
            RangeStart = 1,
            RangeEnd = 2,
            Label = "Build",
            Instructions = "Make the thing."
        }, user.Id);

        var detail = await service.GetByIdAsync(pattern.Id, user.Id);

        Assert.NotNull(detail);
        Assert.True(detail.Progress.HasPieces);
        Assert.True(detail.Progress.HasSteps);
        Assert.True(detail.Progress.IsReadyForProject);
        Assert.Empty(detail.Progress.ValidationMessages);
    }
}
