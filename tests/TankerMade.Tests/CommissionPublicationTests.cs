using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;
using TankerMade.Core.Enums;
using TankerMade.Modules.Knitting;
using TankerMade.Modules.Knitting.Entities;
using TankerMade.Modules.Knitting.DTOs.Patterns;
using TankerMade.Modules.Knitting.DTOs.Projects;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using TankerMade.Server.Services.Knitting;
using TankerMade.Server.Services.ModuleCapabilities;
using Xunit;

namespace TankerMade.Tests;

public class CommissionPublicationTests
{
    [Fact]
    public async Task Publish_snapshot_omits_private_fields_and_translates_knitting_jargon()
    {
        await using var harness = await Harness.CreateAsync();
        var assetId = Guid.NewGuid();
        harness.Context.AssetRecords.Add(new AssetRecord(
            assetId,
            harness.UserId,
            KnittingModule.ModuleKey,
            "finished.jpg",
            "image/jpeg",
            3,
            "memory",
            "present",
            "project",
            harness.ProjectId));
        await harness.Context.SaveChangesAsync();

        var result = await harness.Service.PublishAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new PublishClientProgressRequest
        {
            Stage = CommissionStage.InProgress,
            MaterialNames = ["Wool"],
            PhotoAssetIds = [assetId, Guid.NewGuid()],
            QuotePrice = 180m,
            DepositReceived = true,
            TargetHourlyRate = 25m,
            MaterialCost = 40m,
            ElapsedSeconds = 5400,
            ImpliedNet = 140m,
            BeatsRate = true,
            PrivateNotes = "Do not show the client the note",
            Measurements = "chest 40 in"
        });

        var stored = harness.Context.CommissionPublications.Single();
        var json = stored.SnapshotJson;

        Assert.Equal(CommissionCommandStatus.Ok, result.Status);
        Assert.Equal("Assembling the pieces.", result.Snapshot!.Summary);
        Assert.Equal(180m, result.Snapshot!.Price);
        Assert.Null(stored.HostedAt);
        Assert.Equal(stored.LastPublishedAt, result.Snapshot.LastUpdated);
        Assert.DoesNotContain("depositReceived", json, StringComparison.OrdinalIgnoreCase);
        Assert.Equal([assetId], result.Snapshot.PhotoAssetIds);
        Assert.DoesNotContain("targetHourlyRate", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("elapsedSeconds", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("privateNotes", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("measurements", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("materialCost", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("impliedNet", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("beatsRate", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("quotePrice", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("depositReceived", json, StringComparison.OrdinalIgnoreCase);
        Assert.False(stored.TokenHash == result.IssuedToken);
        Assert.Equal(Sha256Hex(result.IssuedToken!), stored.TokenHash);

        var publicProperties = typeof(ClientProgressSnapshotDto).GetProperties().Select(property => property.Name).ToArray();
        Assert.DoesNotContain(publicProperties, name => name.Contains("Rate", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicProperties, name => name.Contains("Note", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicProperties, name => name.Contains("Measurement", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicProperties, name => name.Contains("Elapsed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AddRevision_records_a_price_change_and_a_due_date_change()
    {
        await using var harness = await Harness.CreateAsync();
        await harness.Service.PublishAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new PublishClientProgressRequest
        {
            Stage = CommissionStage.Accepted,
            PublicSummary = "The piece is underway."
        });

        var priced = await harness.Service.AddRevisionAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new AddClientProgressRevisionRequest
        {
            Summary = "Extra length",
            PreviousPrice = 180m,
            NewPrice = 220m
        });
        var dated = await harness.Service.AddRevisionAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new AddClientProgressRevisionRequest
        {
            Summary = "Due date moved",
            PreviousDueDate = new DateOnly(2026, 10, 1),
            NewDueDate = new DateOnly(2026, 10, 20)
        });

        var rows = harness.Context.CommissionRevisions.OrderBy(revision => revision.OccurredAt).ToList();

        Assert.Equal(CommissionCommandStatus.Ok, priced.Status);
        Assert.True(rows[0].PriceChanged);
        Assert.False(rows[0].DueDateChanged);
        Assert.Equal(220m, rows[0].NewPrice);
        Assert.True(rows[1].DueDateChanged);
        Assert.False(rows[1].PriceChanged);
        Assert.Equal(new DateOnly(2026, 10, 20), rows[1].NewDueDate);
        Assert.Equal(2, dated.Snapshot!.Revisions.Count);
        Assert.Equal(CommissionStage.Revision, dated.Snapshot.Stage);
        Assert.Equal(220m, dated.Snapshot.Price);
        Assert.Equal(new DateOnly(2026, 10, 20), dated.Snapshot.DueDate);
    }

    [Fact]
    public async Task Workshop_keeps_economics_private_and_publish_stays_local()
    {
        await using var harness = await Harness.CreateAsync();
        var yarn = new KnittingYarnInventoryItem(Guid.NewGuid(), harness.UserId, "Brook", "Fog");
        yarn.MergeDetails("blue", "worsted", "wool", "wool", 1, null, "yd", null, 12m, false);
        harness.Context.KnittingYarnInventoryItems.Add(yarn);
        harness.Context.KnittingProjectInventoryLinks.Add(new KnittingProjectInventoryLink(
            Guid.NewGuid(),
            harness.ProjectId,
            "yarn",
            yarn.Id,
            2,
            string.Empty));
        var stepId = harness.Context.KnittingPatternSteps.Select(step => step.Id).First();
        var timer = new KnittingProjectTimer(Guid.NewGuid(), harness.ProjectId, stepId);
        timer.SetElapsedSeconds(3600, DateTime.UtcNow);
        harness.Context.KnittingProjectTimers.Add(timer);
        var user = harness.Context.Users.Single(item => item.Id == harness.UserId);
        user.SetTargetHourlyRate(20m);
        await harness.Context.SaveChangesAsync();

        await harness.Service.SaveTermsAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new UpdateCommissionTermsRequest
        {
            QuotePrice = 40m,
            DueDate = new DateOnly(2026, 11, 1),
            DepositReceived = true
        });

        var workshop = await harness.Service.GetWorkshopAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId);
        var published = await harness.Service.PublishAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new PublishClientProgressRequest
        {
            Stage = CommissionStage.Materials,
            PublicSummary = "Materials are in hand.",
            QuotePrice = 40m,
            DueDate = new DateOnly(2026, 11, 1),
            DepositReceived = true
        });

        Assert.NotNull(workshop);
        Assert.Equal(24m, workshop.MaterialCost);
        Assert.Equal(3600, workshop.ElapsedSeconds);
        Assert.Equal(16m, workshop.ImpliedNet);
        Assert.Equal(16m, workshop.ImpliedHourly);
        Assert.False(workshop.BeatsRate);
        Assert.Equal("Assembling the pieces.", workshop.SuggestedSummary);
        Assert.Equal(40m, published.Snapshot!.Price);
        Assert.DoesNotContain("targetHourlyRate", harness.Context.CommissionPublications.Single().SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("impliedNet", harness.Context.CommissionPublications.Single().SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.Null(harness.Context.CommissionPublications.Single().HostedAt);
    }

    [Fact]
    public async Task Publish_is_rejected_when_knitting_is_inactive()
    {
        using var factory = new DbContextTestFactory();
        await using var context = factory.CreateContext();
        var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
        var service = new CommissionPublicationService(
            context,
            moduleService,
            [new KnittingClientStatusProvider(context)],
            new MemoryAssetStorage());

        var result = await service.PublishAsync(user.Id, KnittingModule.ModuleKey, Guid.NewGuid(), new PublishClientProgressRequest
        {
            Stage = CommissionStage.Quote
        });

        Assert.Equal(CommissionCommandStatus.ModuleInactive, result.Status);
        Assert.Empty(context.CommissionPublications);
    }

    [Fact]
    public async Task Revoked_token_does_not_resolve_and_unpublished_photo_does_not_open()
    {
        await using var harness = await Harness.CreateAsync();
        var publishedId = Guid.NewGuid();
        var hiddenId = Guid.NewGuid();
        harness.Context.AssetRecords.Add(new AssetRecord(
            publishedId,
            harness.UserId,
            KnittingModule.ModuleKey,
            "show.jpg",
            "image/jpeg",
            3,
            "memory",
            "present",
            "project",
            harness.ProjectId));
        harness.Context.AssetRecords.Add(new AssetRecord(
            hiddenId,
            harness.UserId,
            KnittingModule.ModuleKey,
            "hide.jpg",
            "image/jpeg",
            3,
            "memory",
            "hidden",
            "project",
            harness.ProjectId));
        await harness.Context.SaveChangesAsync();

        var published = await harness.Service.PublishAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId, new PublishClientProgressRequest
        {
            Stage = CommissionStage.Ready,
            PublicSummary = "Ready for handoff.",
            PhotoAssetIds = [publishedId]
        });

        var before = await harness.Service.GetByTokenAsync(published.IssuedToken);
        var photo = await harness.Service.OpenPhotoAsync(published.IssuedToken, publishedId);
        var hidden = await harness.Service.OpenPhotoAsync(published.IssuedToken, hiddenId);

        await harness.Service.RevokeAsync(harness.UserId, KnittingModule.ModuleKey, harness.ProjectId);
        var after = await harness.Service.GetByTokenAsync(published.IssuedToken);
        var unknown = await harness.Service.GetByTokenAsync("not-a-real-token");

        Assert.NotNull(before);
        Assert.NotNull(photo);
        Assert.Equal(3, photo.Content.Length);
        Assert.Null(hidden);
        Assert.Null(after);
        Assert.Null(unknown);
        photo?.Content.Dispose();
    }

    private static string Sha256Hex(string token)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private sealed class Harness : IAsyncDisposable
    {
        private readonly DbContextTestFactory _factory;
        public TankerMade.Server.Data.TankerMadeDbContext Context { get; }
        public CommissionPublicationService Service { get; }
        public Guid UserId { get; }
        public Guid ProjectId { get; }

        private Harness(DbContextTestFactory factory, TankerMade.Server.Data.TankerMadeDbContext context, CommissionPublicationService service, Guid userId, Guid projectId)
        {
            _factory = factory;
            Context = context;
            Service = service;
            UserId = userId;
            ProjectId = projectId;
        }

        public static async Task<Harness> CreateAsync()
        {
            var factory = new DbContextTestFactory();
            var context = factory.CreateContext();
            var user = new User(Guid.NewGuid(), "maker", "maker@example.test", "hash");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var moduleService = new ModuleService(context, [new BundledModuleDiscoveryProvider()]);
            await moduleService.ActivateAsync(KnittingModule.ModuleKey, user.Id);

            var patterns = new KnittingPatternService(context);
            var projects = new KnittingProjectService(context);
            var pattern = await patterns.CreateAsync(new CreateKnittingPatternDto { Name = "Pullover" }, user.Id);
            var piece = await patterns.AddPieceAsync(pattern.Id, new CreateKnittingPatternPieceDto { Name = "Body" }, user.Id);
            await patterns.AddStepAsync(pattern.Id, piece!.Id, new CreateKnittingPatternStepDto
            {
                Label = "Blocking the body",
                Instructions = "Wet block"
            }, user.Id);
            var project = await projects.CreateAsync(new CreateKnittingProjectDto
            {
                Name = "Blue pullover",
                PatternId = pattern.Id
            }, user.Id);

            var service = new CommissionPublicationService(
                context,
                moduleService,
                [new KnittingClientStatusProvider(context)],
                new MemoryAssetStorage());

            return new Harness(factory, context, service, user.Id, project.Id);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            _factory.Dispose();
        }
    }

    private sealed class MemoryAssetStorage : IAssetStorageService
    {
        public string ProviderName => "memory";

        public Task<StoredAssetFileResult> StoreAsync(StoreAssetFileRequest request, Stream content, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<Stream?> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            Stream? stream = storagePath == "present" ? new MemoryStream([1, 2, 3]) : null;
            return Task.FromResult(stream);
        }

        public Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
            => Task.FromResult(false);
    }
}
