using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.Services;
using TankerMade.Core.Entities;

namespace TankerMade.Server.Services.Assets;

public class AssetThumbnailService : IAssetThumbnailService
{
    private static readonly (string SizeKey, int MaxWidth, int MaxHeight)[] ThumbnailSizes =
    [
        ("small", 240, 240),
        ("medium", 640, 640)
    ];

    private readonly IAssetStorageService _assetStorageService;

    public AssetThumbnailService(IAssetStorageService assetStorageService)
    {
        _assetStorageService = assetStorageService;
    }

    public async Task<IReadOnlyList<AssetThumbnail>> GenerateAsync(
        AssetRecord asset,
        CancellationToken cancellationToken = default)
    {
        if (!IsImageContentType(asset.ContentType))
        {
            return [];
        }

        await using var sourceStream = await _assetStorageService.OpenReadAsync(asset.StoragePath, cancellationToken);
        if (sourceStream == null)
        {
            return [];
        }

        Image image;
        try
        {
            image = await Image.LoadAsync(sourceStream, cancellationToken);
        }
        catch (Exception)
        {
            return [];
        }

        using var loadedImage = image;
        var results = new List<AssetThumbnail>();

        foreach (var thumbSize in ThumbnailSizes)
        {
            using var clone = loadedImage.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(thumbSize.MaxWidth, thumbSize.MaxHeight),
                    Sampler = KnownResamplers.Bicubic
                });
            });

            await using var output = new MemoryStream();
            await clone.SaveAsJpegAsync(output, new JpegEncoder
            {
                Quality = 85
            }, cancellationToken);

            output.Position = 0;
            var stored = await _assetStorageService.StoreAsync(
                new StoreAssetFileRequest
                {
                    AssetId = asset.Id,
                    UserId = asset.UserId,
                    ModuleKey = asset.ModuleKey,
                    OriginalFileName = $"{asset.Id:N}-{thumbSize.SizeKey}.jpg",
                    VariantKey = thumbSize.SizeKey
                },
                output,
                cancellationToken);

            results.Add(new AssetThumbnail(
                Guid.NewGuid(),
                asset.Id,
                thumbSize.SizeKey,
                "image/jpeg",
                clone.Width,
                clone.Height,
                stored.StorageProvider,
                stored.StoragePath,
                stored.FileSizeBytes));
        }

        return results;
    }

    public Task<Stream?> OpenReadAsync(
        AssetThumbnail thumbnail,
        CancellationToken cancellationToken = default)
    {
        return _assetStorageService.OpenReadAsync(thumbnail.StoragePath, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        AssetThumbnail thumbnail,
        CancellationToken cancellationToken = default)
    {
        return _assetStorageService.DeleteAsync(thumbnail.StoragePath, cancellationToken);
    }

    private static bool IsImageContentType(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }
}
