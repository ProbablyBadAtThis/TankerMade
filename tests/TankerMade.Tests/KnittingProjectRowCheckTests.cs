using TankerMade.Core.Entities;
using TankerMade.Modules.Knitting.DTOs.Patterns;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Server.Services.Knitting;
using Xunit;

namespace TankerMade.Tests;

public class KnittingProjectRowCheckTests
{
    [Fact]
    public async Task SetRowCheckAsync_persists_a_row_and_reloads_it()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        var other = new User(Guid.NewGuid(), "other", "other@example.test", "hash");
        context.Users.AddRange(user, other);
        await context.SaveChangesAsync();

        var patterns = new KnittingPatternService(context);
        var projects = new KnittingProjectService(context);
        var pattern = await patterns.CreateAsync(new CreateKnittingPatternDto { Name = "Hat" }, user.Id);
        var piece = await patterns.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto { Name = "Body" }, user.Id);
        var step = await patterns.AddStepAsync(pattern.Id, piece!.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 5,
            RangeEnd = 7,
            Instructions = "Knit"
        }, user.Id);
        var project = await projects.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Blue hat",
            PatternId = pattern.Id
        }, user.Id);

        var checkedProject = await projects.SetRowCheckAsync(project.Id, step!.Id, 6, new UpdateKnittingProjectRowCheckDto
        {
            IsChecked = true
        }, user.Id);

        var reloaded = await projects.GetByIdAsync(project.Id, user.Id);
        var hidden = await projects.SetRowCheckAsync(project.Id, step.Id, 6, new UpdateKnittingProjectRowCheckDto
        {
            IsChecked = false
        }, other.Id);

        Assert.NotNull(checkedProject);
        Assert.Contains(checkedProject.RowChecks, check => check.PatternStepId == step.Id && check.RowNumber == 6);
        Assert.NotNull(reloaded);
        Assert.Contains(reloaded.RowChecks, check => check.PatternStepId == step.Id && check.RowNumber == 6);
        Assert.Null(hidden);
    }

    [Fact]
    public async Task SetStepProgressAsync_checks_every_row_and_clears_them_when_reopened()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var patterns = new KnittingPatternService(context);
        var projects = new KnittingProjectService(context);
        var pattern = await patterns.CreateAsync(new CreateKnittingPatternDto { Name = "Scarf" }, user.Id);
        var piece = await patterns.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto { Name = "Length" }, user.Id);
        var step = await patterns.AddStepAsync(pattern.Id, piece!.Id, new CreateKnittingPatternStepDto
        {
            RangeStart = 1,
            RangeEnd = 3
        }, user.Id);
        var project = await projects.CreateAsync(new CreateKnittingProjectDto
        {
            Name = "Grey scarf",
            PatternId = pattern.Id
        }, user.Id);

        var completed = await projects.SetStepProgressAsync(project.Id, step!.Id, new UpdateKnittingProjectStepProgressDto
        {
            IsComplete = true
        }, user.Id);
        var reopened = await projects.SetStepProgressAsync(project.Id, step.Id, new UpdateKnittingProjectStepProgressDto
        {
            IsComplete = false
        }, user.Id);

        Assert.NotNull(completed);
        Assert.Equal([1, 2, 3], completed.RowChecks.Select(check => check.RowNumber).Order().ToArray());
        Assert.NotNull(reopened);
        Assert.Empty(reopened.RowChecks);
        Assert.DoesNotContain(reopened.StepProgress, progress => progress.PatternStepId == step.Id && progress.IsComplete);
    }
}
