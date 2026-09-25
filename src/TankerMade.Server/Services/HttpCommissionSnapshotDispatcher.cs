using System.Text;

namespace TankerMade.Server.Services;

public sealed class HttpCommissionSnapshotDispatcher : ICommissionSnapshotDispatcher
{
    private readonly HttpClient _http;

    public HttpCommissionSnapshotDispatcher(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> TryDeliverAsync(
        Guid publicationId,
        string? issuedToken,
        string snapshotJson,
        IReadOnlyList<CommissionSnapshotPhoto> photos,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"snapshots/{publicationId}");
        request.Content = new StringContent(snapshotJson, Encoding.UTF8, "application/json");
        if (!string.IsNullOrWhiteSpace(issuedToken))
        {
            request.Headers.TryAddWithoutValidation("X-Client-Token", issuedToken);
        }

        using var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        foreach (var photo in photos)
        {
            using var photoRequest = new HttpRequestMessage(HttpMethod.Put, $"snapshots/{publicationId}/photos/{photo.AssetId:D}");
            photoRequest.Content = new ByteArrayContent(photo.Content);
            photoRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
            using var photoResponse = await _http.SendAsync(photoRequest, cancellationToken);
            if (!photoResponse.IsSuccessStatusCode)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> TryRevokeAsync(Guid publicationId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.DeleteAsync($"snapshots/{publicationId}", cancellationToken);
        return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound;
    }
}
