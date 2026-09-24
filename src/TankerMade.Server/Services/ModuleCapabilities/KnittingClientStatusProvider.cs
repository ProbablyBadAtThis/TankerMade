using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Core.Enums;
using TankerMade.Modules.Knitting;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services.ModuleCapabilities;

public class KnittingClientStatusProvider : IModuleClientStatusProvider
{
    private readonly TankerMadeDbContext _context;

    public KnittingClientStatusProvider(TankerMadeDbContext context)
    {
        _context = context;
    }

    public string ModuleKey => KnittingModule.ModuleKey;

    public async Task<ClientProgressSnapshotDto?> BuildSnapshotAsync(
        Guid userId,
        Guid projectId,
        ClientProgressPublishChoices choices,
        IReadOnlyList<ClientProgressRevisionDto> revisions,
        DateTime publishedAt,
        CancellationToken cancellationToken = default)
    {
        var project = await _context.KnittingProjects
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == projectId && item.UserId == userId && !item.IsArchived, cancellationToken);

        if (project == null)
        {
            return null;
        }

        var steps = new List<StepSentence>();
        if (project.PatternId.HasValue)
        {
            var rows = await (
                from step in _context.KnittingPatternSteps.AsNoTracking()
                join piece in _context.KnittingPatternPieces.AsNoTracking() on step.PatternPieceId equals piece.Id
                where piece.PatternId == project.PatternId
                orderby piece.SortOrder, step.SortOrder
                select new { step.Id, step.Label, step.Instructions, PieceName = piece.Name })
                .ToListAsync(cancellationToken);

            var completeIds = await _context.KnittingProjectStepProgress
                .AsNoTracking()
                .Where(progress => progress.ProjectId == project.Id && progress.IsComplete)
                .Select(progress => progress.PatternStepId)
                .ToListAsync(cancellationToken);

            steps = rows
                .Select(row => new StepSentence(row.Id, ToPublicSentence(row.Label, row.Instructions, row.PieceName), completeIds.Contains(row.Id)))
                .ToList();
        }

        var current = steps.FirstOrDefault(step => !step.IsComplete) ?? steps.LastOrDefault();
        var next = current == null
            ? null
            : steps.SkipWhile(step => step.Id != current.Id).Skip(1).FirstOrDefault();

        var summary = FirstText(choices.PublicSummary, current?.Sentence, "In progress.");
        var nextStep = FirstText(choices.NextStep, next?.Sentence, "Waiting on the next update.");
        var photoIds = await PublishedPhotoIdsAsync(userId, projectId, choices.PhotoAssetIds, cancellationToken);

        return new ClientProgressSnapshotDto
        {
            Stage = choices.Stage,
            Summary = summary,
            MaterialLines = CleanNames(choices.MaterialNames)
                .Select(name => new ClientProgressMaterialLineDto { Name = name })
                .ToList(),
            PhotoAssetIds = photoIds,
            NextStep = nextStep,
            Title = project.Name,
            Price = choices.Price,
            DueDate = choices.DueDate,
            LastUpdated = publishedAt,
            Revisions = revisions.ToList()
        };
    }

    public async Task<ClientProgressWorkFacts?> GetWorkFactsAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await _context.KnittingProjects
            .AsNoTracking()
            .AnyAsync(item => item.Id == projectId && item.UserId == userId && !item.IsArchived, cancellationToken);

        if (!projectExists)
        {
            return null;
        }

        var links = await _context.KnittingProjectInventoryLinks
            .AsNoTracking()
            .Where(link => link.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        decimal materialCost = 0;
        var names = new List<string>();
        foreach (var link in links)
        {
            var (name, price) = await GetInventoryCostAsync(link.InventoryItemType, link.InventoryItemId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(name))
            {
                names.Add(name);
            }

            if (price.HasValue)
            {
                var quantity = link.QuantityPlanned is > 0 ? link.QuantityPlanned.Value : 1m;
                materialCost += price.Value * quantity;
            }
        }

        var timers = await _context.KnittingProjectTimers
            .AsNoTracking()
            .Where(timer => timer.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        var asOf = DateTime.UtcNow;
        var elapsed = timers.Sum(timer => timer.GetElapsedSeconds(asOf));

        return new ClientProgressWorkFacts
        {
            MaterialCost = materialCost,
            ElapsedSeconds = elapsed,
            MaterialNames = names
        };
    }

    private async Task<(string Name, decimal? Price)> GetInventoryCostAsync(
        string itemType,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        if (itemType == "yarn")
        {
            var yarn = await _context.KnittingYarnInventoryItems.AsNoTracking()
                .Where(item => item.Id == itemId)
                .Select(item => new { Name = item.BrandName + " " + item.ColorName, item.RegularPrice })
                .SingleOrDefaultAsync(cancellationToken);
            return yarn == null ? (string.Empty, null) : (yarn.Name, yarn.RegularPrice);
        }

        if (itemType == "tool")
        {
            var tool = await _context.KnittingToolInventoryItems.AsNoTracking()
                .Where(item => item.Id == itemId)
                .Select(item => new { Name = item.BrandName + " " + item.TypeName, item.RegularPrice })
                .SingleOrDefaultAsync(cancellationToken);
            return tool == null ? (string.Empty, null) : (tool.Name, tool.RegularPrice);
        }

        if (itemType == "notion")
        {
            var notion = await _context.KnittingNotionInventoryItems.AsNoTracking()
                .Where(item => item.Id == itemId)
                .Select(item => new { Name = item.BrandName + " " + item.TypeName, item.RegularPrice })
                .SingleOrDefaultAsync(cancellationToken);
            return notion == null ? (string.Empty, null) : (notion.Name, notion.RegularPrice);
        }

        return (string.Empty, null);
    }

    private async Task<List<Guid>> PublishedPhotoIdsAsync(
        Guid userId,
        Guid projectId,
        IReadOnlyList<Guid> requestedIds,
        CancellationToken cancellationToken)
    {
        if (requestedIds.Count == 0)
        {
            return [];
        }

        var allowed = await _context.AssetRecords
            .AsNoTracking()
            .Where(asset => asset.UserId == userId
                && !asset.IsDeleted
                && asset.ModuleKey == ModuleKey
                && asset.RecordId == projectId
                && requestedIds.Contains(asset.Id))
            .Select(asset => asset.Id)
            .ToListAsync(cancellationToken);

        return requestedIds.Where(allowed.Contains).Distinct().ToList();
    }

    internal static string ToPublicSentence(string? label, string? instructions, string? pieceName)
    {
        var source = $"{label} {instructions}";
        if (source.Contains("block", StringComparison.OrdinalIgnoreCase))
        {
            return "Assembling the pieces.";
        }

        if (!string.IsNullOrWhiteSpace(pieceName))
        {
            return $"Working on the {pieceName.Trim()}.";
        }

        return "In progress.";
    }

    private static List<string> CleanNames(IEnumerable<string>? names)
    {
        return (names ?? [])
            .Select(name => name?.Trim() ?? string.Empty)
            .Where(name => name.Length > 0)
            .Select(name => name.Length <= 80 ? name : name[..80])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(20)
            .ToList();
    }

    private static string FirstText(string? preferred, string? fallback, string final)
    {
        foreach (var candidate in new[] { preferred, fallback, final })
        {
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                var trimmed = candidate.Trim();
                return trimmed.Length <= 500 ? trimmed : trimmed[..500];
            }
        }

        return final;
    }

    private sealed record StepSentence(Guid Id, string Sentence, bool IsComplete);
}
