using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingDashboardContributionProvider : IModuleDashboardContributionProvider
{
    private readonly TankerMadeDbContext _context;

    public KnittingDashboardContributionProvider(TankerMadeDbContext context)
    {
        _context = context;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<ModuleDashboardContributionDto> GetContributionAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeProjectCount = await _context.KnittingProjects
            .AsNoTracking()
            .CountAsync(
                project => project.UserId == userId && !project.IsArchived && project.Progress < 100,
                cancellationToken);

        return new ModuleDashboardContributionDto
        {
            ActiveProjectCount = activeProjectCount,
            QuickActions =
            [
                new DashboardQuickActionDto
                {
                    ModuleKey = ModuleKey,
                    Label = "Open Knitting",
                    NavigationPath = "/modules/knitting",
                    SortOrder = 10,
                },
                new DashboardQuickActionDto
                {
                    ModuleKey = ModuleKey,
                    Label = "New project",
                    NavigationPath = "/modules/knitting/projects/new",
                    SortOrder = 20,
                },
                new DashboardQuickActionDto
                {
                    ModuleKey = ModuleKey,
                    Label = "Inventory",
                    NavigationPath = "/modules/knitting/inventory",
                    SortOrder = 30,
                },
            ],
            DueSoonItems = [],
        };
    }
}
