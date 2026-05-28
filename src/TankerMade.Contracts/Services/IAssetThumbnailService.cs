using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Core.Entities;

namespace TankerMade.Contracts.Services;

public interface IAssetThumbnailService
{
    Task<IReadOnlyList<AssetThumbnail>> GenerateAsync(
        AssetRecord asset,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        AssetThumbnail thumbnail,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        AssetThumbnail thumbnail,
        CancellationToken cancellationToken = default);
}
