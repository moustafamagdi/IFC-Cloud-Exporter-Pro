using ACCIFCConverter.Domain.Contracts;

namespace ACCIFCConverter.Infrastructure.Storage;

public sealed class LocalFileTransferService : IFileTransferService
{
    public async Task<string> DownloadAsync(string sourceFileId, string targetDirectory, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(targetDirectory);
        var path = Path.Combine(targetDirectory, sourceFileId.Replace(':', '_') + ".rvt");
        await File.WriteAllTextAsync(path, "Mock RVT download", cancellationToken);
        return path;
    }

    public Task<string> UploadToAccAsync(string localFilePath, string destinationFolderId, CancellationToken cancellationToken = default)
    {
        // placeholder for APS Data Management upload
        return Task.FromResult($"acc://{destinationFolderId}/{Path.GetFileName(localFilePath)}");
    }
}
