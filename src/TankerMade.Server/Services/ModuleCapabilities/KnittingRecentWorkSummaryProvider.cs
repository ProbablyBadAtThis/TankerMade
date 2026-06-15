using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingRecentWorkSummaryProvider : IModuleRecentWorkSummaryProvider
{
    private const string ProjectRecordType = "project";

    private readonly TankerMadeDbContext _context;

    public KnittingRecentWorkSummaryProvider(TankerMadeDbContext context)
    {
        _context = context;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<IReadOnlyList<RecentWorkSummaryDto>> GetSummariesAsync(
        Guid userId,
        IReadOnlyList<RecentWorkItemRef> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
        {
            return [];
        }

        var projectRefs = items
            .Where(item => string.Equals(item.WorkItemType, RecentWorkTypes.Project, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (projectRefs.Count == 0)
        {
            return [];
        }

        var projectIds = projectRefs.Select(item => item.WorkItemId).Distinct().ToList();
        var projects = await _context.KnittingProjects
            .AsNoTracking()
            .Where(project => project.UserId == userId && projectIds.Contains(project.Id))
            .Select(project => new { project.Id, project.Name })
            .ToDictionaryAsync(project => project.Id, cancellationToken);

        var thumbnailAssets = await _context.AssetRecords
            .AsNoTracking()
            .Where(asset => asset.UserId == userId
                && !asset.IsDeleted
                && asset.ModuleKey == ModuleKey
                && asset.RecordType == ProjectRecordType
                && asset.RecordId.HasValue
                && projectIds.Contains(asset.RecordId.Value))
            .OrderBy(asset => asset.CreatedAt)
            .Select(asset => new { ProjectId = asset.RecordId!.Value, asset.Id })
            .ToListAsync(cancellationToken);

        var thumbnailByProjectId = thumbnailAssets
            .GroupBy(asset => asset.ProjectId)
            .ToDictionary(group => group.Key, group => group.First().Id);

        var summaries = new List<RecentWorkSummaryDto>();
        foreach (var itemRef in projectRefs)
        {
            if (!projects.TryGetValue(itemRef.WorkItemId, out var project))
            {
                continue;
            }

            Guid? thumbnailAssetId = thumbnailByProjectId.TryGetValue(itemRef.WorkItemId, out var assetId)
                ? assetId
                : null;

            summaries.Add(new RecentWorkSummaryDto
            {
                ModuleKey = ModuleKey,
                WorkItemType = RecentWorkTypes.Project,
                WorkItemId = itemRef.WorkItemId,
                Title = project.Name,
                ThumbnailAssetId = thumbnailAssetId,
                ThumbnailFallbackPath = KnittingModule.DefaultProjectThumbnailPath,
                LastAccessedAtUtc = itemRef.LastAccessedAtUtc,
                NavigationPath = $"/modules/knitting/projects/{itemRef.WorkItemId}",
            });
        }

        return summaries;
    }
}
