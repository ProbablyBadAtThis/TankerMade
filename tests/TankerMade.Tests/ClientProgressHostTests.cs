extern alias Host;

using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TankerMade.Tests;

public class ClientProgressHostTests : IAsyncLifetime
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "tm-host-" + Guid.NewGuid().ToString("N"));
    private WebApplicationFactory<Host::Program> _factory = null!;
    private HttpClient _client = null!;

    public Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Host::Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Host:ApiKey", "test-key");
            builder.UseSetting("Host:DataDirectory", _root);
        });
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Hosted_page_shows_the_snapshot_hides_private_fields_and_dies_on_revoke()
    {
        var publicationId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var token = "public-token";
        var snapshot = $$"""
            {
              "stage": "InProgress",
              "title": "Blue pullover",
              "summary": "Assembling the pieces.",
              "materialLines": [{ "name": "Wool" }],
              "photoAssetIds": ["{{photoId}}"],
              "nextStep": "Waiting on the next update.",
              "price": 180,
              "dueDate": "2026-10-20",
              "lastUpdated": "2026-09-24T22:00:00Z",
              "revisions": [{ "occurredAt": "2026-09-24T22:00:00Z", "summary": "Added a pocket", "priceChanged": true, "previousPrice": 160, "newPrice": 180 }],
              "privateNotes": "secret-note",
              "targetHourlyRate": 25,
              "elapsedSeconds": 5400,
              "measurements": "chest 40 in"
            }
            """;

        using var publish = new HttpRequestMessage(HttpMethod.Put, $"/snapshots/{publicationId}")
        {
            Content = new StringContent(snapshot, System.Text.Encoding.UTF8, "application/json")
        };
        publish.Headers.TryAddWithoutValidation("X-Host-Key", "test-key");
        publish.Headers.TryAddWithoutValidation("X-Client-Token", token);
        Assert.Equal(HttpStatusCode.NoContent, (await _client.SendAsync(publish)).StatusCode);

        using var photo = new HttpRequestMessage(HttpMethod.Put, $"/snapshots/{publicationId}/photos/{photoId:D}")
        {
            Content = new ByteArrayContent([9, 8, 7])
        };
        photo.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        photo.Headers.TryAddWithoutValidation("X-Host-Key", "test-key");
        Assert.Equal(HttpStatusCode.NoContent, (await _client.SendAsync(photo)).StatusCode);

        var page = await _client.GetAsync($"/client-progress/{token}");
        var html = await page.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        Assert.Contains("Blue pullover", html);
        Assert.Contains("In progress", html);
        Assert.Contains("Assembling the pieces.", html);
        Assert.Contains("Wool", html);
        Assert.Contains("$180.00", html);
        Assert.Contains("Added a pocket", html);
        Assert.Contains($"/client-progress/{token}/photos/{photoId:D}", html);
        Assert.DoesNotContain("secret-note", html);
        Assert.DoesNotContain("targetHourlyRate", html);
        Assert.DoesNotContain("elapsedSeconds", html);
        Assert.DoesNotContain("chest 40", html);

        var image = await _client.GetAsync($"/client-progress/{token}/photos/{photoId:D}");
        Assert.Equal(HttpStatusCode.OK, image.StatusCode);
        Assert.Equal("image/jpeg", image.Content.Headers.ContentType?.MediaType);
        Assert.Equal(new byte[] { 9, 8, 7 }, await image.Content.ReadAsByteArrayAsync());
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/client-progress/{token}/photos/{Guid.NewGuid():D}")).StatusCode);

        using var revoke = new HttpRequestMessage(HttpMethod.Delete, $"/snapshots/{publicationId}");
        revoke.Headers.TryAddWithoutValidation("X-Host-Key", "test-key");
        Assert.Equal(HttpStatusCode.NoContent, (await _client.SendAsync(revoke)).StatusCode);

        var revoked = await _client.GetAsync($"/client-progress/{token}");
        var revokedHtml = await revoked.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, revoked.StatusCode);
        Assert.Contains("This link is not available", revokedHtml);
        Assert.DoesNotContain("Blue pullover", revokedHtml);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/client-progress/{token}/photos/{photoId:D}")).StatusCode);
    }

    [Fact]
    public async Task Writes_without_the_host_key_are_rejected()
    {
        var response = await _client.PutAsync($"/snapshots/{Guid.NewGuid()}", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
