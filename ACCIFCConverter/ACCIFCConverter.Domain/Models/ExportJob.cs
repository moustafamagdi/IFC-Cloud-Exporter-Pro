namespace ACCIFCConverter.Domain.Models;

public enum ExportJobStatus { Waiting, Uploading, Processing, Completed, Failed }

public sealed class ExportJob
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string HubId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string SourceFileId { get; set; } = string.Empty;
    public string SourceFileName { get; set; } = string.Empty;
    public string DestinationFolderId { get; set; } = string.Empty;
    public string OutputFileName { get; set; } = string.Empty;
    public ExportJobStatus Status { get; set; } = ExportJobStatus.Waiting;
    public string? Preset { get; set; }
    public string? JsonConfigPath { get; set; }
    public string? MappingFilePath { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public string? ResultUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
