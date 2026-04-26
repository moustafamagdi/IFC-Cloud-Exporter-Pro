namespace ACCIFCConverter.Domain.Models;

public sealed class ExportHistoryRecord
{
    public long Id { get; set; }
    public Guid JobId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset StartUtc { get; set; }
    public DateTimeOffset EndUtc { get; set; }
    public long DurationSeconds { get; set; }
    public string Result { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string Logs { get; set; } = string.Empty;
}
