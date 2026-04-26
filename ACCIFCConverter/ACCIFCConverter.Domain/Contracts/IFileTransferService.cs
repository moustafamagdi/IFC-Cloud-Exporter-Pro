namespace ACCIFCConverter.Domain.Contracts;

public interface IFileTransferService
{
    Task<string> DownloadAsync(string sourceFileId, string targetDirectory, CancellationToken cancellationToken = default);
    Task<string> UploadToAccAsync(string localFilePath, string destinationFolderId, CancellationToken cancellationToken = default);
}
