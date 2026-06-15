using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Dashboard;
using TankerMade.Contracts.Services;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Core.Entities;
using TankerMade.Server.Data;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Services;

public class RecentWorkService : IRecentWorkService
{
    private const int MaxStoredEntriesPerUser = 50;

    private readonly TankerMadeDbContext _context;
    private readonly IModuleService _moduleService;
    private readonly IModuleRecentWorkSummaryResolver _summaryResolver;

    public RecentWorkService(
        TankerMadeDbContext context,
        IModuleService moduleService,
        IModuleRecentWorkSummaryResolver summaryResolver)
    {
        _context = context;
        _moduleService = moduleService;
        _summaryResolver = summaryResolver;
    }

    public async Task RecordAccessAsync(
        Guid userId,
        RecordRecentWorkRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var moduleKey = request.ModuleKey?.Trim() ?? string.Empty;
        var workItemType = request.WorkItemType?.Trim() ?? string.Empty;
        if (moduleKey.Length == 0 || workItemType.Length == 0 || request.WorkItemId == Guid.Empty)
        {
            throw new ArgumentException("ModuleKey, WorkItemType, and WorkItemId are required.");
        }

        if (!await _moduleService.IsActiveAsync(moduleKey, userId))
        {
            return;
        }

        var accessedAt = DateTime.UtcNow;
        var existing = await _context.UserRecentWorkAccesses
            .SingleOrDefaultAsync(
                entry => entry.UserId == userId
                    && entry.ModuleKey == moduleKey
                    && entry.WorkItemType == workItemType
                    && entry.WorkItemId == request.WorkItemId,
                cancellationToken);

        if (existing == null)
        {
            _context.UserRecentWorkAccesses.Add(new UserRecentWorkAccess(
                Guid.NewGuid(),
                userId,
                moduleKey,
                workItemType,
                request.WorkItemId,
                accessedAt));
        }
        else
        {
            existing.Touch(accessedAt);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await TrimStoredEntriesAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyList<RecentWorkSummaryDto>> GetRecentAsync(
        Guid userId,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        if (limit <= 0)
        {
            return [];
        }

        var activeModuleKeys = (await _moduleService.GetActiveModulesAsync(userId))
            .Select(module => module.ModuleKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (activeModuleKeys.Count == 0)
        {
            return [];
        }

        var ledgerEntries = await _context.UserRecentWorkAccesses
            .AsNoTracking()
            .Where(entry => entry.UserId == userId && activeModuleKeys.Contains(entry.ModuleKey))
            .OrderByDescending(entry => entry.LastAccessedAtUtc)
            .Take(MaxStoredEntriesPerUser)
            .ToListAsync(cancellationToken);

        if (ledgerEntries.Count == 0)
        {
            return [];
        }

        var summaries = new List<RecentWorkSummaryDto>();
        foreach (var moduleGroup in ledgerEntries.GroupBy(entry => entry.ModuleKey, StringComparer.OrdinalIgnoreCase))
        {
            var provider = _summaryResolver.Resolve(moduleGroup.Key);
            if (provider == null)
            {
                continue;
            }

            var refs = moduleGroup
                .Select(entry => new RecentWorkItemRef
                {
                    ModuleKey = entry.ModuleKey,
                    WorkItemType = entry.WorkItemType,
                    WorkItemId = entry.WorkItemId,
                    LastAccessedAtUtc = entry.LastAccessedAtUtc,
                })
                .ToList();

            var moduleSummaries = await provider.GetSummariesAsync(userId, refs, cancellationToken);
            summaries.AddRange(moduleSummaries);
        }

        return summaries
            .OrderByDescending(summary => summary.LastAccessedAtUtc)
            .Take(limit)
            .ToList();
    }

    private async Task TrimStoredEntriesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var overflow = await _context.UserRecentWorkAccesses
            .Where(entry => entry.UserId == userId)
            .OrderByDescending(entry => entry.LastAccessedAtUtc)
            .Skip(MaxStoredEntriesPerUser)
            .ToListAsync(cancellationToken);

        if (overflow.Count == 0)
        {
            return;
        }

        _context.UserRecentWorkAccesses.RemoveRange(overflow);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
