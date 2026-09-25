using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Core.Entities;
using TankerMade.Modules.Knitting;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using TankerMade.Server.Services.ModuleCapabilities;
using Xunit;

namespace TankerMade.Tests;

public class RecentWorkServiceTests
{
    [Fact]
    public async Task RecordAccessAsync_upserts_and_GetRecentAsync_returns_provider_summary()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
        await moduleService.ActivateAsync(KnittingModule.ModuleKey, user.Id);

        var projectId = Guid.NewGuid();
        var provider = new StubRecentWorkSummaryProvider(KnittingModule.ModuleKey, projectId, "Test Scarf");
        var resolver = new ModuleRecentWorkSummaryResolver([provider]);
        var service = new RecentWorkService(context, moduleService, resolver);

        await service.RecordAccessAsync(user.Id, new RecordRecentWorkRequest
        {
            ModuleKey = KnittingModule.ModuleKey,
            WorkItemType = RecentWorkTypes.Project,
            WorkItemId = projectId,
        });

        var recent = await service.GetRecentAsync(user.Id, limit: 5);

        Assert.Single(recent);
        Assert.Equal("Test Scarf", recent[0].Title);
        Assert.Equal(projectId, recent[0].WorkItemId);
        Assert.Equal($"/modules/knitting/projects/{projectId}", recent[0].NavigationPath);
    }

    [Fact]
    public async Task GetRecentAsync_skips_entries_for_inactive_modules()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
        var projectId = Guid.NewGuid();
        context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
            Guid.NewGuid(),
            user.Id,
            KnittingModule.ModuleKey,
            RecentWorkTypes.Project,
            projectId,
            DateTime.UtcNow));
        await context.SaveChangesAsync();

        var provider = new StubRecentWorkSummaryProvider(KnittingModule.ModuleKey, projectId, "Hidden Project");
        var resolver = new ModuleRecentWorkSummaryResolver([provider]);
        var service = new RecentWorkService(context, moduleService, resolver);

        var recent = await service.GetRecentAsync(user.Id, limit: 5);

        Assert.Empty(recent);
    }

    private sealed class StubRecentWorkSummaryProvider : IModuleRecentWorkSummaryProvider
    {
        private readonly Guid _projectId;
        private readonly string _title;

        public StubRecentWorkSummaryProvider(string moduleKey, Guid projectId, string title)
        {
            ModuleKey = moduleKey;
            _projectId = projectId;
            _title = title;
        }

        public string ModuleKey { get; }

        public Task<IReadOnlyList<RecentWorkSummaryDto>> GetSummariesAsync(
            Guid userId,
            IReadOnlyList<RecentWorkItemRef> items,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<RecentWorkSummaryDto> summaries = items
                .Where(item => item.WorkItemId == _projectId)
                .Select(item => new RecentWorkSummaryDto
                {
                    ModuleKey = ModuleKey,
                    WorkItemType = item.WorkItemType,
                    WorkItemId = item.WorkItemId,
                    Title = _title,
                    LastAccessedAtUtc = item.LastAccessedAtUtc,
                    NavigationPath = $"/modules/{ModuleKey}/projects/{item.WorkItemId}",
                })
                .ToList();

            return Task.FromResult(summaries);
        }
    }
}
