using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.ModuleCapabilities;

internal static class KnittingStudioAttentionBuilder
{
    private const string YarnType = "yarn";
    private const string ToolType = "tool";
    private const string NotionType = "notion";
    private const int MaxItems = 8;

    public static async Task<IReadOnlyList<DashboardAttentionItemDto>> BuildAsync(
        TankerMadeDbContext context,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var items = new List<DashboardAttentionItemDto>();
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var activeProjects = await context.KnittingProjects
            .AsNoTracking()
            .Where(project => project.UserId == userId && !project.IsArchived && project.Progress < 100)
            .Select(project => new { project.Id, project.Name })
            .ToListAsync(cancellationToken);

        if (activeProjects.Count > 0)
        {
            var projectIds = activeProjects.Select(project => project.Id).ToList();
            var projectNames = activeProjects.ToDictionary(project => project.Id, project => project.Name);
            var links = await context.KnittingProjectInventoryLinks
                .AsNoTracking()
                .Where(link => projectIds.Contains(link.ProjectId))
                .ToListAsync(cancellationToken);

            if (links.Count > 0)
            {
                var yarnIds = links.Where(link => link.InventoryItemType == YarnType).Select(link => link.InventoryItemId).Distinct().ToList();
                var toolIds = links.Where(link => link.InventoryItemType == ToolType).Select(link => link.InventoryItemId).Distinct().ToList();
                var notionIds = links.Where(link => link.InventoryItemType == NotionType).Select(link => link.InventoryItemId).Distinct().ToList();

                var yarns = yarnIds.Count == 0
                    ? new Dictionary<Guid, KnittingYarnInventoryItem>()
                    : await context.KnittingYarnInventoryItems
                        .AsNoTracking()
                        .Where(item => item.UserId == userId && yarnIds.Contains(item.Id))
                        .ToDictionaryAsync(item => item.Id, cancellationToken);

                var tools = toolIds.Count == 0
                    ? new Dictionary<Guid, KnittingToolInventoryItem>()
                    : await context.KnittingToolInventoryItems
                        .AsNoTracking()
                        .Where(item => item.UserId == userId && toolIds.Contains(item.Id))
                        .ToDictionaryAsync(item => item.Id, cancellationToken);

                var notions = notionIds.Count == 0
                    ? new Dictionary<Guid, KnittingNotionInventoryItem>()
                    : await context.KnittingNotionInventoryItems
                        .AsNoTracking()
                        .Where(item => item.UserId == userId && notionIds.Contains(item.Id))
                        .ToDictionaryAsync(item => item.Id, cancellationToken);

                foreach (var link in links)
                {
                    if (items.Count >= MaxItems)
                    {
                        break;
                    }

                    if (!projectNames.TryGetValue(link.ProjectId, out var projectName))
                    {
                        continue;
                    }

                    var attention = EvaluateProjectLink(link, projectName, yarns, tools, notions);
                    if (attention == null || !seenKeys.Add(attention.Key))
                    {
                        continue;
                    }

                    items.Add(new DashboardAttentionItemDto
                    {
                        ModuleKey = KnittingModule.ModuleKey,
                        Title = attention.Title,
                        Detail = attention.Detail,
                        NavigationPath = attention.NavigationPath,
                        SortOrder = 10,
                    });
                }
            }
        }

        if (items.Count < MaxItems)
        {
            var lowYarns = await context.KnittingYarnInventoryItems
                .AsNoTracking()
                .Where(item => item.UserId == userId
                    && (item.TotalSkeins < 1m
                        || (item.EstimatedRemainingLength.HasValue && item.EstimatedRemainingLength.Value <= 0m)))
                .OrderBy(item => item.TotalSkeins)
                .ThenBy(item => item.BrandName)
                .Take(MaxItems)
                .ToListAsync(cancellationToken);

            foreach (var yarn in lowYarns)
            {
                if (items.Count >= MaxItems)
                {
                    break;
                }

                var key = $"yarn:{yarn.Id}";
                if (!seenKeys.Add(key))
                {
                    continue;
                }

                var label = $"{yarn.BrandName} — {yarn.ColorName}";
                var detail = yarn.TotalSkeins < 1m
                    ? "Less than one skein on hand"
                    : "Remaining length is depleted";

                items.Add(new DashboardAttentionItemDto
                {
                    ModuleKey = KnittingModule.ModuleKey,
                    Title = label,
                    Detail = detail,
                    NavigationPath = $"/modules/knitting/inventory/yarn/{yarn.Id}",
                    SortOrder = 20,
                });
            }
        }

        return items;
    }

    private static AttentionCandidate? EvaluateProjectLink(
        KnittingProjectInventoryLink link,
        string projectName,
        IReadOnlyDictionary<Guid, KnittingYarnInventoryItem> yarns,
        IReadOnlyDictionary<Guid, KnittingToolInventoryItem> tools,
        IReadOnlyDictionary<Guid, KnittingNotionInventoryItem> notions)
    {
        return link.InventoryItemType switch
        {
            YarnType when yarns.TryGetValue(link.InventoryItemId, out var yarn) =>
                EvaluateYarnLink(link, projectName, yarn),
            ToolType when tools.TryGetValue(link.InventoryItemId, out var tool) =>
                EvaluateQuantityLink(
                    link,
                    projectName,
                    $"{tool.BrandName} — {tool.TypeName}",
                    tool.Quantity,
                    $"/modules/knitting/inventory/tool/{tool.Id}"),
            NotionType when notions.TryGetValue(link.InventoryItemId, out var notion) =>
                EvaluateQuantityLink(
                    link,
                    projectName,
                    $"{notion.BrandName} — {notion.TypeName}",
                    notion.Quantity,
                    $"/modules/knitting/inventory/notion/{notion.Id}"),
            YarnType or ToolType or NotionType =>
                new AttentionCandidate(
                    $"link-missing:{link.Id}",
                    projectName,
                    "Linked inventory item is missing",
                    "Update project supplies",
                    $"/modules/knitting/projects/{link.ProjectId}"),
            _ => null,
        };
    }

    private static AttentionCandidate? EvaluateYarnLink(
        KnittingProjectInventoryLink link,
        string projectName,
        KnittingYarnInventoryItem yarn)
    {
        var label = $"{yarn.BrandName} — {yarn.ColorName}";

        if (yarn.TotalSkeins < 1m)
        {
            return new AttentionCandidate(
                $"project-yarn:{link.ProjectId}:{yarn.Id}:skeins",
                projectName,
                $"{label} is out of skeins",
                "Linked yarn for an active project",
                $"/modules/knitting/projects/{link.ProjectId}");
        }

        if (yarn.EstimatedRemainingLength is <= 0m)
        {
            return new AttentionCandidate(
                $"project-yarn:{link.ProjectId}:{yarn.Id}:length",
                projectName,
                $"{label} has no remaining length",
                "Linked yarn for an active project",
                $"/modules/knitting/inventory/yarn/{yarn.Id}");
        }

        if (link.QuantityPlanned is > 0m && yarn.TotalSkeins < link.QuantityPlanned.Value)
        {
            return new AttentionCandidate(
                $"project-yarn:{link.ProjectId}:{yarn.Id}:planned",
                projectName,
                $"{label} below planned amount",
                $"Need {link.QuantityPlanned:0.##} skeins, have {yarn.TotalSkeins:0.##}",
                $"/modules/knitting/projects/{link.ProjectId}");
        }

        return null;
    }

    private static AttentionCandidate? EvaluateQuantityLink(
        KnittingProjectInventoryLink link,
        string projectName,
        string label,
        int quantityOnHand,
        string _)
    {
        if (quantityOnHand <= 0)
        {
            return new AttentionCandidate(
                $"project-{link.InventoryItemType}:{link.ProjectId}:{link.InventoryItemId}:zero",
                projectName,
                $"{label} is out of stock",
                "Linked supply for an active project",
                $"/modules/knitting/projects/{link.ProjectId}");
        }

        if (link.QuantityPlanned is > 0m && quantityOnHand < link.QuantityPlanned.Value)
        {
            return new AttentionCandidate(
                $"project-{link.InventoryItemType}:{link.ProjectId}:{link.InventoryItemId}:planned",
                projectName,
                $"{label} below planned amount",
                $"Need {link.QuantityPlanned:0.##}, have {quantityOnHand}",
                $"/modules/knitting/projects/{link.ProjectId}");
        }

        return null;
    }

    private sealed record AttentionCandidate(
        string Key,
        string Title,
        string Detail,
        string SubDetail,
        string NavigationPath);
}
