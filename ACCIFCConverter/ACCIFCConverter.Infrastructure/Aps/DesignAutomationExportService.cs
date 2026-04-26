using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Infrastructure.Aps;

public sealed class DesignAutomationExportService : IExportExecutionService
{
    public async Task<ExportJob> ExecuteAsync(ExportJob job, ExportOptions options, CancellationToken cancellationToken = default)
    {
        job.Status = ExportJobStatus.Processing;
        await Task.Delay(1200, cancellationToken);

        var suffix = options.UseJsonConfig ? "JSON" : options.Preset.Replace(" ", string.Empty);
        job.ResultUrl = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(job.SourceFileName)}_{suffix}.ifc");
        await File.WriteAllTextAsync(job.ResultUrl, $"IFC mock output for {job.SourceFileName}", cancellationToken);

        job.Status = ExportJobStatus.Completed;
        job.CompletedAt = DateTimeOffset.UtcNow;
        return job;
    }
}
