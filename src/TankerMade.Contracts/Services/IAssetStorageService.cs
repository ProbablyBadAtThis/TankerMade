using TankerMade.Contracts.DTOs.Assets;

namespace TankerMade.Contracts.Services;

public interface IAssetStorageService
{
    string ProviderName { get; }

    Task<StoredAssetFileResult> StoreAsync(
        StoreAssetFileRequest request,
        Stream content,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken = default);
}
