using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Core.Entities;
using TankerMade.Server.Data;

namespace TankerMade.Server.Services;

public class CommissionPublicationService : ICommissionPublicationService
{
    private static readonly JsonSerializerOptions SnapshotJson = new(JsonSerializerDefaults.Web);

    private readonly TankerMadeDbContext _context;
    private readonly IModuleService _moduleService;
    private readonly IReadOnlyDictionary<string, IModuleClientStatusProvider> _providers;
    private readonly IAssetStorageService _assetStorage;
    private readonly ICommissionSnapshotDispatcher _dispatcher;
    private readonly IConfiguration? _configuration;

    public CommissionPublicationService(
        TankerMadeDbContext context,
        IModuleService moduleService,
        IEnumerable<IModuleClientStatusProvider> providers,
        IAssetStorageService assetStorage,
        ICommissionSnapshotDispatcher? dispatcher = null,
        IConfiguration? configuration = null)
    {
        _context = context;
        _moduleService = moduleService;
        _providers = providers.ToDictionary(provider => provider.ModuleKey, StringComparer.OrdinalIgnoreCase);
        _assetStorage = assetStorage;
        _dispatcher = dispatcher ?? new UnconfiguredCommissionSnapshotDispatcher();
        _configuration = configuration;
    }

    public async Task<CommissionCommandResult> PublishAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        PublishClientProgressRequest request,
        CancellationToken cancellationToken = default)
    {
        var gate = await GateAsync(userId, moduleKey, cancellationToken);
        if (gate != null)
        {
            return gate;
        }

        var provider = Resolve(moduleKey);
        if (provider == null)
        {
            return Missing();
        }

        var publishedAt = DateTime.UtcNow;
        var workspace = await SaveWorkspaceAsync(userId, moduleKey, projectId, request.QuotePrice, request.DueDate, request.DepositReceived, publishedAt, cancellationToken);
        var existing = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        var revisions = existing == null
            ? []
            : await LoadRevisionsAsync(existing.Id, cancellationToken);

        if (request.Revision != null)
        {
            revisions.Add(ToRevisionDto(request.Revision, publishedAt));
        }

        var choices = ToChoices(request);
        choices.Price = workspace.QuotePrice;
        choices.DueDate = workspace.DueDate;
        var snapshot = await provider.BuildSnapshotAsync(
            userId,
            projectId,
            choices,
            revisions,
            publishedAt,
            cancellationToken);

        if (snapshot == null)
        {
            return Missing();
        }

        var json = JsonSerializer.Serialize(snapshot, SnapshotJson);
        string? issuedToken = null;
        CommissionPublication publication;

        if (existing == null)
        {
            issuedToken = CreateToken();
            publication = new CommissionPublication(
                Guid.NewGuid(),
                userId,
                moduleKey,
                projectId,
                HashToken(issuedToken),
                json,
                publishedAt);
            _context.CommissionPublications.Add(publication);
        }
        else if (existing.IsRevoked)
        {
            issuedToken = CreateToken();
            existing.RotateToken(HashToken(issuedToken), json, publishedAt);
            publication = existing;
        }
        else
        {
            existing.ReplaceSnapshot(json, publishedAt);
            publication = existing;
        }

        if (request.Revision != null)
        {
            _context.CommissionRevisions.Add(ToRevisionEntity(publication.Id, request.Revision, publishedAt));
        }

        await _context.SaveChangesAsync(cancellationToken);
        var hosted = await TryMarkHostedAsync(publication, issuedToken, json, publishedAt, cancellationToken);
        return Ok(snapshot, issuedToken, hosted ? HostedPage(issuedToken) : null);
    }

    public async Task<CommissionCommandResult> AddRevisionAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        AddClientProgressRevisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var gate = await GateAsync(userId, moduleKey, cancellationToken);
        if (gate != null)
        {
            return gate;
        }

        var publication = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        if (publication == null || publication.IsRevoked)
        {
            return Missing();
        }

        var provider = Resolve(moduleKey);
        if (provider == null)
        {
            return Missing();
        }

        var occurredAt = DateTime.UtcNow;
        var revisions = await LoadRevisionsAsync(publication.Id, cancellationToken);
        var input = new ClientProgressRevisionInput
        {
            Summary = request.Summary,
            PreviousPrice = request.PreviousPrice,
            NewPrice = request.NewPrice,
            PreviousDueDate = request.PreviousDueDate,
            NewDueDate = request.NewDueDate
        };
        revisions.Add(ToRevisionDto(input, occurredAt));

        var current = JsonSerializer.Deserialize<ClientProgressSnapshotDto>(publication.SnapshotJson, SnapshotJson);
        var price = request.NewPrice ?? current?.Price;
        var dueDate = request.NewDueDate ?? current?.DueDate;
        var existingTerms = await FindWorkspaceAsync(userId, moduleKey, projectId, cancellationToken);
        var terms = await SaveWorkspaceAsync(
            userId,
            moduleKey,
            projectId,
            price,
            dueDate,
            existingTerms?.DepositReceived ?? false,
            occurredAt,
            cancellationToken);
        var snapshot = await provider.BuildSnapshotAsync(
            userId,
            projectId,
            new ClientProgressPublishChoices
            {
                Stage = Core.Enums.CommissionStage.Revision,
                PublicSummary = current?.Summary,
                MaterialNames = current?.MaterialLines.Select(line => line.Name).ToList() ?? [],
                PhotoAssetIds = current?.PhotoAssetIds ?? [],
                NextStep = current?.NextStep,
                Price = terms.QuotePrice,
                DueDate = terms.DueDate
            },
            revisions,
            occurredAt,
            cancellationToken);

        if (snapshot == null)
        {
            return Missing();
        }

        snapshot.Stage = Core.Enums.CommissionStage.Revision;
        var revisionJson = JsonSerializer.Serialize(snapshot, SnapshotJson);
        publication.ReplaceSnapshot(revisionJson, occurredAt);
        _context.CommissionRevisions.Add(ToRevisionEntity(publication.Id, input, occurredAt));
        await _context.SaveChangesAsync(cancellationToken);
        await TryMarkHostedAsync(publication, null, revisionJson, occurredAt, cancellationToken);
        return Ok(snapshot, null);
    }

    public async Task<CommissionCommandResult> RevokeAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var gate = await GateAsync(userId, moduleKey, cancellationToken);
        if (gate != null)
        {
            return gate;
        }

        var publication = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        if (publication == null || publication.IsRevoked)
        {
            return Missing();
        }

        publication.Revoke(DateTime.UtcNow);
        await _context.SaveChangesAsync(cancellationToken);
        try
        {
            await _dispatcher.TryRevokeAsync(publication.Id, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
        }

        return new CommissionCommandResult { Status = CommissionCommandStatus.Ok };
    }

    public async Task<CommissionWorkshopDto?> GetWorkshopAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        if (!await _moduleService.IsActiveAsync(moduleKey, userId))
        {
            return null;
        }

        var provider = Resolve(moduleKey);
        if (provider == null)
        {
            return null;
        }

        var facts = await provider.GetWorkFactsAsync(userId, projectId, cancellationToken);
        if (facts == null)
        {
            return null;
        }

        var workspace = await FindWorkspaceAsync(userId, moduleKey, projectId, cancellationToken);
        var publication = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        var rate = await _context.Users.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.TargetHourlyRate)
            .SingleOrDefaultAsync(cancellationToken);

        var draft = await provider.BuildSnapshotAsync(
            userId,
            projectId,
            new ClientProgressPublishChoices
            {
                Stage = Core.Enums.CommissionStage.InProgress,
                MaterialNames = facts.MaterialNames,
                Price = workspace?.QuotePrice,
                DueDate = workspace?.DueDate
            },
            [],
            DateTime.UtcNow,
            cancellationToken);

        decimal? impliedNet = workspace?.QuotePrice is decimal quote
            ? quote - facts.MaterialCost
            : null;
        decimal? impliedHourly = impliedNet is decimal net && facts.ElapsedSeconds > 0
            ? decimal.Round(net / (facts.ElapsedSeconds / 3600m), 2, MidpointRounding.AwayFromZero)
            : null;

        return new CommissionWorkshopDto
        {
            TargetHourlyRate = rate,
            QuotePrice = workspace?.QuotePrice,
            DueDate = workspace?.DueDate,
            DepositReceived = workspace?.DepositReceived ?? false,
            MaterialCost = facts.MaterialCost,
            ElapsedSeconds = facts.ElapsedSeconds,
            ImpliedNet = impliedNet,
            ImpliedHourly = impliedHourly,
            BeatsRate = impliedHourly is decimal hourly && rate is decimal target ? hourly >= target : null,
            SuggestedSummary = draft?.Summary ?? string.Empty,
            SuggestedNextStep = draft?.NextStep ?? string.Empty,
            SuggestedMaterialNames = facts.MaterialNames,
            HasActiveLink = publication is { IsRevoked: false },
            LastPublishedAt = publication is { IsRevoked: false } ? publication.LastPublishedAt : null
        };
    }

    public async Task<CommissionCommandResult> SaveTermsAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        UpdateCommissionTermsRequest request,
        CancellationToken cancellationToken = default)
    {
        var gate = await GateAsync(userId, moduleKey, cancellationToken);
        if (gate != null)
        {
            return gate;
        }

        var provider = Resolve(moduleKey);
        if (provider == null || await provider.GetWorkFactsAsync(userId, projectId, cancellationToken) == null)
        {
            return Missing();
        }

        await SaveWorkspaceAsync(userId, moduleKey, projectId, request.QuotePrice, request.DueDate, request.DepositReceived, DateTime.UtcNow, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new CommissionCommandResult { Status = CommissionCommandStatus.Ok };
    }

    public async Task<CommissionCommandResult> ReissueLinkAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var gate = await GateAsync(userId, moduleKey, cancellationToken);
        if (gate != null)
        {
            return gate;
        }

        var publication = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        if (publication == null || publication.IsRevoked)
        {
            return Missing();
        }

        var token = CreateToken();
        publication.ReplaceToken(HashToken(token));
        await _context.SaveChangesAsync(cancellationToken);
        var hosted = await TryMarkHostedAsync(publication, token, publication.SnapshotJson, publication.LastPublishedAt, cancellationToken);
        return new CommissionCommandResult
        {
            Status = CommissionCommandStatus.Ok,
            IssuedToken = token,
            HostedUrl = hosted ? HostedPage(token) : null,
            Snapshot = JsonSerializer.Deserialize<ClientProgressSnapshotDto>(publication.SnapshotJson, SnapshotJson)
        };
    }

    public async Task<MakerRateDto?> GetMakerRateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rate = await _context.Users.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => (decimal?)user.TargetHourlyRate)
            .SingleOrDefaultAsync(cancellationToken);

        return rate == null && !await _context.Users.AnyAsync(user => user.Id == userId, cancellationToken)
            ? null
            : new MakerRateDto { TargetHourlyRate = rate };
    }

    public async Task<MakerRateDto?> SetMakerRateAsync(Guid userId, decimal? targetHourlyRate, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.SingleOrDefaultAsync(item => item.Id == userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        user.SetTargetHourlyRate(targetHourlyRate);
        await _context.SaveChangesAsync(cancellationToken);
        return new MakerRateDto { TargetHourlyRate = user.TargetHourlyRate };
    }

    public async Task<ClientProgressSnapshotDto?> GetPreviewAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        if (!await _moduleService.IsActiveAsync(moduleKey, userId))
        {
            return null;
        }

        var publication = await FindPublicationAsync(userId, moduleKey, projectId, cancellationToken);
        if (publication == null || publication.IsRevoked)
        {
            return null;
        }

        return JsonSerializer.Deserialize<ClientProgressSnapshotDto>(publication.SnapshotJson, SnapshotJson);
    }

    public async Task<ClientProgressSnapshotDto?> GetByTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        var publication = await FindActiveByTokenAsync(token, cancellationToken);
        if (publication == null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<ClientProgressSnapshotDto>(publication.SnapshotJson, SnapshotJson);
    }

    public async Task<PublishedClientPhoto?> OpenPhotoAsync(string? token, Guid assetId, CancellationToken cancellationToken = default)
    {
        var publication = await FindActiveByTokenAsync(token, cancellationToken);
        if (publication == null)
        {
            return null;
        }

        var snapshot = JsonSerializer.Deserialize<ClientProgressSnapshotDto>(publication.SnapshotJson, SnapshotJson);
        if (snapshot == null || !snapshot.PhotoAssetIds.Contains(assetId))
        {
            return null;
        }

        var asset = await _context.AssetRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == assetId && item.UserId == publication.UserId && !item.IsDeleted, cancellationToken);

        if (asset == null)
        {
            return null;
        }

        var stream = await _assetStorage.OpenReadAsync(asset.StoragePath, cancellationToken);
        if (stream == null)
        {
            return null;
        }

        return new PublishedClientPhoto
        {
            Content = stream,
            ContentType = asset.ContentType
        };
    }

    private async Task<CommissionWorkspace> SaveWorkspaceAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        decimal? quotePrice,
        DateOnly? dueDate,
        bool depositReceived,
        DateTime updatedAt,
        CancellationToken cancellationToken)
    {
        var workspace = await FindWorkspaceAsync(userId, moduleKey, projectId, cancellationToken);
        if (workspace == null)
        {
            workspace = new CommissionWorkspace(Guid.NewGuid(), userId, moduleKey, projectId, updatedAt);
            _context.CommissionWorkspaces.Add(workspace);
        }

        workspace.Update(quotePrice, dueDate, depositReceived, updatedAt);
        return workspace;
    }

    private Task<CommissionWorkspace?> FindWorkspaceAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var key = moduleKey.Trim();
        return _context.CommissionWorkspaces
            .SingleOrDefaultAsync(item => item.UserId == userId && item.ModuleKey == key && item.ProjectId == projectId, cancellationToken);
    }

    private async Task<IReadOnlyList<CommissionSnapshotPhoto>?> TryLoadPublishedPhotosAsync(
        Guid userId,
        string snapshotJson,
        CancellationToken cancellationToken)
    {
        var snapshot = JsonSerializer.Deserialize<ClientProgressSnapshotDto>(snapshotJson, SnapshotJson);
        if (snapshot == null)
        {
            return null;
        }

        var photos = new List<CommissionSnapshotPhoto>();
        foreach (var assetId in snapshot.PhotoAssetIds.Distinct())
        {
            var asset = await _context.AssetRecords
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == assetId && item.UserId == userId && !item.IsDeleted, cancellationToken);
            if (asset == null)
            {
                return null;
            }

            var stream = await _assetStorage.OpenReadAsync(asset.StoragePath, cancellationToken);
            if (stream == null)
            {
                return null;
            }

            await using (stream)
            {
                using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer, cancellationToken);
                photos.Add(new CommissionSnapshotPhoto(assetId, asset.ContentType, buffer.ToArray()));
            }
        }

        return photos;
    }

    private string? HostedPage(string? token)
    {
        var baseUrl = _configuration?["ClientProgressHost:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return baseUrl.TrimEnd('/') + "/client-progress/" + token;
    }

    private async Task<bool> TryMarkHostedAsync(
        CommissionPublication publication,
        string? issuedToken,
        string snapshotJson,
        DateTime publishedAt,
        CancellationToken cancellationToken)
    {
        var photos = await TryLoadPublishedPhotosAsync(publication.UserId, snapshotJson, cancellationToken);
        if (photos == null)
        {
            return false;
        }

        bool delivered;
        try
        {
            delivered = await _dispatcher.TryDeliverAsync(publication.Id, issuedToken, snapshotJson, photos, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            delivered = false;
        }

        if (!delivered)
        {
            return false;
        }

        publication.MarkHosted(publishedAt);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<CommissionCommandResult?> GateAsync(Guid userId, string moduleKey, CancellationToken cancellationToken)
    {
        if (!await _moduleService.IsActiveAsync(moduleKey, userId))
        {
            return new CommissionCommandResult { Status = CommissionCommandStatus.ModuleInactive };
        }

        return null;
    }

    private IModuleClientStatusProvider? Resolve(string moduleKey)
    {
        return _providers.TryGetValue(moduleKey.Trim(), out var provider) ? provider : null;
    }

    private Task<CommissionPublication?> FindPublicationAsync(
        Guid userId,
        string moduleKey,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var key = moduleKey.Trim();
        return _context.CommissionPublications
            .SingleOrDefaultAsync(item => item.UserId == userId && item.ModuleKey == key && item.ProjectId == projectId, cancellationToken);
    }

    private async Task<CommissionPublication?> FindActiveByTokenAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var hash = HashToken(token.Trim());
        var publication = await _context.CommissionPublications
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.TokenHash == hash && item.RevokedAt == null, cancellationToken);

        return publication;
    }

    private async Task<List<ClientProgressRevisionDto>> LoadRevisionsAsync(Guid publicationId, CancellationToken cancellationToken)
    {
        return await _context.CommissionRevisions
            .AsNoTracking()
            .Where(revision => revision.PublicationId == publicationId)
            .OrderBy(revision => revision.OccurredAt)
            .Select(revision => new ClientProgressRevisionDto
            {
                OccurredAt = revision.OccurredAt,
                Summary = revision.Summary,
                PriceChanged = revision.PriceChanged,
                DueDateChanged = revision.DueDateChanged,
                PreviousPrice = revision.PreviousPrice,
                NewPrice = revision.NewPrice,
                PreviousDueDate = revision.PreviousDueDate,
                NewDueDate = revision.NewDueDate
            })
            .ToListAsync(cancellationToken);
    }

    private static ClientProgressPublishChoices ToChoices(PublishClientProgressRequest request)
    {
        return new ClientProgressPublishChoices
        {
            Stage = request.Stage,
            PublicSummary = request.PublicSummary,
            MaterialNames = request.MaterialNames,
            PhotoAssetIds = request.PhotoAssetIds,
            NextStep = request.NextStep,
            Price = request.QuotePrice,
            DueDate = request.DueDate
        };
    }

    private static ClientProgressRevisionDto ToRevisionDto(ClientProgressRevisionInput input, DateTime occurredAt)
    {
        var (priceChanged, dueDateChanged) = ChangeFlags(input);
        return new ClientProgressRevisionDto
        {
            OccurredAt = occurredAt,
            Summary = input.Summary?.Trim() ?? string.Empty,
            PriceChanged = priceChanged,
            DueDateChanged = dueDateChanged,
            PreviousPrice = input.PreviousPrice,
            NewPrice = input.NewPrice,
            PreviousDueDate = input.PreviousDueDate,
            NewDueDate = input.NewDueDate
        };
    }

    private static CommissionRevision ToRevisionEntity(Guid publicationId, ClientProgressRevisionInput input, DateTime occurredAt)
    {
        var (priceChanged, dueDateChanged) = ChangeFlags(input);
        return new CommissionRevision(
            Guid.NewGuid(),
            publicationId,
            occurredAt,
            input.Summary ?? string.Empty,
            priceChanged,
            dueDateChanged,
            input.PreviousPrice,
            input.NewPrice,
            input.PreviousDueDate,
            input.NewDueDate);
    }

    private static (bool PriceChanged, bool DueDateChanged) ChangeFlags(ClientProgressRevisionInput input)
    {
        var priceChanged = input.PreviousPrice != input.NewPrice
            && (input.PreviousPrice.HasValue || input.NewPrice.HasValue);
        var dueDateChanged = input.PreviousDueDate != input.NewDueDate
            && (input.PreviousDueDate.HasValue || input.NewDueDate.HasValue);
        return (priceChanged, dueDateChanged);
    }

    private static CommissionCommandResult Ok(ClientProgressSnapshotDto snapshot, string? issuedToken, string? hostedUrl = null)
    {
        return new CommissionCommandResult
        {
            Status = CommissionCommandStatus.Ok,
            Snapshot = snapshot,
            IssuedToken = issuedToken,
            HostedUrl = hostedUrl
        };
    }

    private static CommissionCommandResult Missing()
    {
        return new CommissionCommandResult { Status = CommissionCommandStatus.NotFound };
    }

    private static string CreateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    internal static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
