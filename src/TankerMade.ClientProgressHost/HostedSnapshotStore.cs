using System.Text.Json;
using TankerMade.Contracts.DTOs.ClientProgress;

namespace TankerMade.ClientProgressHost;

public sealed class HostedSnapshotStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _root;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public HostedSnapshotStore(string root)
    {
        _root = root;
    }

    public async Task<bool> SaveAsync(Guid publicationId, string? issuedToken, string snapshotJson, CancellationToken cancellationToken)
    {
        if (snapshotJson.Length > 100_000)
        {
            return false;
        }

        var snapshot = JsonSerializer.Deserialize<ClientProgressSnapshotDto>(snapshotJson, JsonOptions);
        if (snapshot == null)
        {
            return false;
        }

        var publicJson = JsonSerializer.Serialize(snapshot, JsonOptions);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var folder = Folder(publicationId);
            Directory.CreateDirectory(folder);
            await File.WriteAllTextAsync(Path.Combine(folder, "snapshot.json"), publicJson, cancellationToken);
            if (!string.IsNullOrWhiteSpace(issuedToken))
            {
                await ReplaceTokenAsync(publicationId, issuedToken.Trim(), cancellationToken);
            }

            var allowed = snapshot.PhotoAssetIds.ToHashSet();
            var photoFolder = Path.Combine(folder, "photos");
            if (Directory.Exists(photoFolder))
            {
                foreach (var file in Directory.GetFiles(photoFolder))
                {
                    var name = Path.GetFileName(file);
                    if (name.EndsWith(".type", StringComparison.Ordinal))
                    {
                        name = name[..^5];
                    }

                    if (!Guid.TryParse(name, out var assetId) || !allowed.Contains(assetId))
                    {
                        File.Delete(file);
                    }
                }
            }

            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> SavePhotoAsync(Guid publicationId, Guid assetId, string contentType, Stream content, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var snapshot = await ReadSnapshotAsync(publicationId, cancellationToken);
            if (snapshot == null || !snapshot.PhotoAssetIds.Contains(assetId))
            {
                return false;
            }

            var photoFolder = Path.Combine(Folder(publicationId), "photos");
            Directory.CreateDirectory(photoFolder);
            var path = Path.Combine(photoFolder, assetId.ToString("D"));
            await using (var file = File.Create(path))
            {
                await content.CopyToAsync(file, cancellationToken);
            }

            await File.WriteAllTextAsync(path + ".type", contentType, cancellationToken);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> RevokeAsync(Guid publicationId, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var folder = Folder(publicationId);
            var existed = Directory.Exists(folder);
            if (existed)
            {
                Directory.Delete(folder, recursive: true);
            }

            await RemoveTokenAsync(publicationId, cancellationToken);
            return existed;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<HostedPublication?> FindByTokenAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var index = await ReadIndexAsync(cancellationToken);
            if (!index.TryGetValue(token.Trim(), out var publicationId))
            {
                return null;
            }

            var snapshot = await ReadSnapshotAsync(publicationId, cancellationToken);
            return snapshot == null ? null : new HostedPublication(publicationId, token.Trim(), snapshot);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<(byte[] Content, string ContentType)?> OpenPhotoAsync(string? token, Guid assetId, CancellationToken cancellationToken)
    {
        var publication = await FindByTokenAsync(token, cancellationToken);
        if (publication == null || !publication.Snapshot.PhotoAssetIds.Contains(assetId))
        {
            return null;
        }

        var path = Path.Combine(Folder(publication.PublicationId), "photos", assetId.ToString("D"));
        if (!File.Exists(path))
        {
            return null;
        }

        var typePath = path + ".type";
        var contentType = File.Exists(typePath) ? await File.ReadAllTextAsync(typePath, cancellationToken) : "application/octet-stream";
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
        return (bytes, string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType.Trim());
    }

    private string Folder(Guid publicationId) => Path.Combine(_root, publicationId.ToString("D"));

    private async Task<ClientProgressSnapshotDto?> ReadSnapshotAsync(Guid publicationId, CancellationToken cancellationToken)
    {
        var path = Path.Combine(Folder(publicationId), "snapshot.json");
        if (!File.Exists(path))
        {
            return null;
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<ClientProgressSnapshotDto>(stream, JsonOptions, cancellationToken);
    }

    private async Task ReplaceTokenAsync(Guid publicationId, string token, CancellationToken cancellationToken)
    {
        var index = await ReadIndexAsync(cancellationToken);
        foreach (var existing in index.Where(pair => pair.Value == publicationId).Select(pair => pair.Key).ToList())
        {
            index.Remove(existing);
        }

        index[token] = publicationId;
        await WriteIndexAsync(index, cancellationToken);
    }

    private async Task RemoveTokenAsync(Guid publicationId, CancellationToken cancellationToken)
    {
        var index = await ReadIndexAsync(cancellationToken);
        var removed = false;
        foreach (var existing in index.Where(pair => pair.Value == publicationId).Select(pair => pair.Key).ToList())
        {
            index.Remove(existing);
            removed = true;
        }

        if (removed)
        {
            await WriteIndexAsync(index, cancellationToken);
        }
    }

    private async Task<Dictionary<string, Guid>> ReadIndexAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(_root, "tokens.json");
        if (!File.Exists(path))
        {
            return new Dictionary<string, Guid>(StringComparer.Ordinal);
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<Dictionary<string, Guid>>(stream, JsonOptions, cancellationToken)
            ?? new Dictionary<string, Guid>(StringComparer.Ordinal);
    }

    private async Task WriteIndexAsync(Dictionary<string, Guid> index, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "tokens.json");
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, index, JsonOptions, cancellationToken);
    }
}

public sealed record HostedPublication(Guid PublicationId, string Token, ClientProgressSnapshotDto Snapshot);
