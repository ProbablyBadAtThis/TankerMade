using System.Text;
using TankerMade.Contracts.DTOs.Assets;
using TankerMade.Contracts.Services;

namespace TankerMade.Server.Services.Assets;

public class LocalDiskAssetStorageService : IAssetStorageService
{
    private const int CopyBufferSize = 81920;
    private readonly string _rootDirectory;

    public LocalDiskAssetStorageService(IWebHostEnvironment environment, AssetStorageOptions options)
    {
        var configuredRoot = string.IsNullOrWhiteSpace(options.RootDirectory)
            ? "App_Data/assets"
            : options.RootDirectory.Trim();

        _rootDirectory = Path.IsPathRooted(configuredRoot)
            ? configuredRoot
            : Path.Combine(environment.ContentRootPath, configuredRoot);

        Directory.CreateDirectory(_rootDirectory);
    }

    public string ProviderName => "local-disk";

    public async Task<StoredAssetFileResult> StoreAsync(
        StoreAssetFileRequest request,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(content);

        var moduleKey = Slugify(request.ModuleKey, fallback: "module");
        var extension = GetSafeExtension(request.OriginalFileName);
        var relativePath = Path.Combine(
            request.UserId.ToString("N"),
            moduleKey,
            DateTime.UtcNow.ToString("yyyy"),
            DateTime.UtcNow.ToString("MM"),
            BuildFileName(request.AssetId, request.VariantKey, extension));

        var absolutePath = ResolveAbsolutePath(relativePath);
        var directory = Path.GetDirectoryName(absolutePath)
            ?? throw new InvalidOperationException("Unable to determine asset directory.");

        Directory.CreateDirectory(directory);

        long bytesWritten = 0;
        await using (var fileStream = new FileStream(
            absolutePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            CopyBufferSize,
            useAsync: true))
        {
            if (content.CanSeek)
            {
                content.Position = 0;
            }

            var buffer = new byte[CopyBufferSize];
            while (true)
            {
                var read = await content.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                if (read == 0)
                {
                    break;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                bytesWritten += read;
            }
        }

        return new StoredAssetFileResult
        {
            StorageProvider = ProviderName,
            StoragePath = ToStoragePath(relativePath),
            FileSizeBytes = bytesWritten
        };
    }

    public Task<Stream?> OpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        var absolutePath = ResolveAbsolutePath(storagePath);
        if (!File.Exists(absolutePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(
            absolutePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            CopyBufferSize,
            useAsync: true);

        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Task.FromResult(false);
        }

        var absolutePath = ResolveAbsolutePath(storagePath);
        if (!File.Exists(absolutePath))
        {
            return Task.FromResult(false);
        }

        File.Delete(absolutePath);
        return Task.FromResult(true);
    }

    private string ResolveAbsolutePath(string storagePath)
    {
        var relativePath = storagePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        var combined = Path.Combine(_rootDirectory, relativePath);
        var absolutePath = Path.GetFullPath(combined);
        var absoluteRoot = Path.GetFullPath(_rootDirectory);

        if (!absolutePath.StartsWith(absoluteRoot, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Storage path resolves outside configured asset root.");
        }

        return absolutePath;
    }

    private static string Slugify(string value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var normalized = value.Trim().ToLowerInvariant();
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
            {
                builder.Append(ch);
            }
            else if (ch == '-' || ch == '_' || ch == '.')
            {
                builder.Append('-');
            }
        }

        return builder.Length == 0 ? fallback : builder.ToString();
    }

    private static string GetSafeExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return ".bin";
        }

        var extension = Path.GetExtension(fileName.Trim());
        if (string.IsNullOrWhiteSpace(extension))
        {
            return ".bin";
        }

        return extension.Length <= 20 ? extension.ToLowerInvariant() : ".bin";
    }

    private static string ToStoragePath(string relativePath)
    {
        return relativePath.Replace('\\', '/');
    }

    private static string BuildFileName(Guid assetId, string variantKey, string extension)
    {
        var suffix = string.IsNullOrWhiteSpace(variantKey)
            ? string.Empty
            : $"-{Slugify(variantKey, "variant")}";

        return $"{assetId:N}{suffix}{extension}";
    }
}
