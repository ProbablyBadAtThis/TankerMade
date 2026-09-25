using System.Security.Cryptography;
using System.Text;
using TankerMade.ClientProgressHost;

var builder = WebApplication.CreateBuilder(args);
var dataDirectory = builder.Configuration["Host:DataDirectory"];
if (string.IsNullOrWhiteSpace(dataDirectory))
{
    dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "hosted");
}

var store = new HostedSnapshotStore(dataDirectory);
var apiKey = builder.Configuration["Host:ApiKey"] ?? string.Empty;
var app = builder.Build();

app.MapPut("/snapshots/{publicationId:guid}", async (Guid publicationId, HttpRequest request, CancellationToken cancellationToken) =>
{
    if (!Authorized(request, apiKey))
    {
        return Results.Unauthorized();
    }

    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    var json = await reader.ReadToEndAsync(cancellationToken);
    var token = request.Headers.TryGetValue("X-Client-Token", out var values) ? values.ToString() : null;
    return await store.SaveAsync(publicationId, token, json, cancellationToken)
        ? Results.NoContent()
        : Results.BadRequest();
});

app.MapPut("/snapshots/{publicationId:guid}/photos/{assetId:guid}", async (Guid publicationId, Guid assetId, HttpRequest request, CancellationToken cancellationToken) =>
{
    if (!Authorized(request, apiKey))
    {
        return Results.Unauthorized();
    }

    if (request.ContentLength > 8_000_000)
    {
        return Results.BadRequest();
    }

    var contentType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType;
    return await store.SavePhotoAsync(publicationId, assetId, contentType, request.Body, cancellationToken)
        ? Results.NoContent()
        : Results.NotFound();
});

app.MapDelete("/snapshots/{publicationId:guid}", async (Guid publicationId, HttpRequest request, CancellationToken cancellationToken) =>
{
    if (!Authorized(request, apiKey))
    {
        return Results.Unauthorized();
    }

    await store.RevokeAsync(publicationId, cancellationToken);
    return Results.NoContent();
});

app.MapGet("/client-progress/{token}", async (string token, CancellationToken cancellationToken) =>
{
    var publication = await store.FindByTokenAsync(token, cancellationToken);
    var html = HostedClientPage.Render(publication?.Snapshot, token);
    return publication == null
        ? Results.Content(html, "text/html", statusCode: StatusCodes.Status404NotFound)
        : Results.Content(html, "text/html");
});

app.MapGet("/client-progress/{token}/photos/{assetId:guid}", async (string token, Guid assetId, CancellationToken cancellationToken) =>
{
    var photo = await store.OpenPhotoAsync(token, assetId, cancellationToken);
    return photo == null ? Results.NotFound() : Results.File(photo.Value.Content, photo.Value.ContentType);
});

app.Run();

static bool Authorized(HttpRequest request, string apiKey)
{
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        return false;
    }

    if (!request.Headers.TryGetValue("X-Host-Key", out var provided))
    {
        return false;
    }

    var expected = Encoding.UTF8.GetBytes(apiKey);
    var actual = Encoding.UTF8.GetBytes(provided.ToString());
    return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
}

public partial class Program;
