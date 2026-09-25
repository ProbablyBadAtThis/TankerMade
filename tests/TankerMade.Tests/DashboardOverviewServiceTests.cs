using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Core.Entities;
using TankerMade.Modules.Knitting;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using TankerMade.Server.Services.ModuleCapabilities;
using Xunit;

namespace TankerMade.Tests;

public class DashboardOverviewServiceTests
{
    [Fact]
    public async Task GetOverviewAsync_aggregates_knitting_contribution_and_engagement()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
        await moduleService.ActivateAsync(KnittingModule.ModuleKey, user.Id);

        context.KnittingProjects.Add(new Modules.Knitting.Entities.KnittingProject(Guid.NewGuid(), "Scarf", user.Id));
        context.KnittingProjects.Add(new Modules.Knitting.Entities.KnittingProject(Guid.NewGuid(), "Hat", user.Id)
        {
            IsArchived = true,
        });
        context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
            Guid.NewGuid(),
            user.Id,
            KnittingModule.ModuleKey,
            RecentWorkTypes.Project,
            Guid.NewGuid(),
            DateTime.UtcNow));
        context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
            Guid.NewGuid(),
            user.Id,
            KnittingModule.ModuleKey,
            RecentWorkTypes.Project,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1)));
        await context.SaveChangesAsync();

        var resolver = new ModuleDashboardContributionResolver([new KnittingDashboardContributionProvider(context)]);
        var service = new DashboardOverviewService(context, moduleService, resolver);

        var overview = await service.GetOverviewAsync(user.Id);

        Assert.Equal(1, overview.Activity.ActiveProjectCount);
        Assert.Equal(1, overview.Activity.ActiveModuleCount);
        Assert.Equal(2, overview.Activity.WorkStreakDays);
        Assert.True(overview.Activity.ActiveDaysThisWeek >= 1);
        Assert.True(overview.Activity.ActiveLocalDates.Count >= 2);
        Assert.Contains(overview.QuickActions, action => action.Label == "New project");
    }

    [Fact]
    public async Task GetOverviewAsync_moduleKey_scopes_activity_to_that_module()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
        await moduleService.ActivateAsync(KnittingModule.ModuleKey, user.Id);
        await moduleService.ActivateAsync("printing3d", user.Id);

        var knittingAccess = DateTime.UtcNow;
        var otherAccess = DateTime.UtcNow.AddDays(-2);
        context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
            Guid.NewGuid(),
            user.Id,
            KnittingModule.ModuleKey,
            RecentWorkTypes.Project,
            Guid.NewGuid(),
            knittingAccess));
        context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
            Guid.NewGuid(),
            user.Id,
            "printing3d",
            RecentWorkTypes.Project,
            Guid.NewGuid(),
            otherAccess));
        await context.SaveChangesAsync();

        var resolver = new ModuleDashboardContributionResolver([new KnittingDashboardContributionProvider(context)]);
        var service = new DashboardOverviewService(context, moduleService, resolver);

        var scoped = await service.GetOverviewAsync(user.Id, KnittingModule.ModuleKey);
        var global = await service.GetOverviewAsync(user.Id);

        Assert.Single(scoped.Activity.ActiveLocalDates);
        Assert.Equal(2, global.Activity.ActiveLocalDates.Count);
    }

    [Fact]
    public void CalculateWorkStreakDays_returns_zero_when_last_activity_is_older_than_yesterday()
    {
        var streak = DashboardOverviewService.CalculateWorkStreakDays(
        [
            DateTime.UtcNow.AddDays(-3),
        ]);

        Assert.Equal(0, streak);
    }

    [Fact]
    public void CalculateActiveDaysThisWeek_counts_distinct_local_days()
    {
        var today = DateTime.Today;
        var activeDays = DashboardOverviewService.CalculateActiveDaysThisWeek(
        [
            today.ToUniversalTime(),
            today.ToUniversalTime().AddHours(2),
            today.AddDays(-1).ToUniversalTime(),
        ]);

        Assert.Equal(2, activeDays);
    }
}
