using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Infrastructure.Aps;

public sealed class DesignAutomationExportService
{
    public async Task<ExportJob> ExecuteAsync(ExportJob job, CancellationToken cancellationToken = default)
    {
        job.Status = ExportJobStatus.Uploading;
        await Task.Delay(300, cancellationToken);
        job.Status = ExportJobStatus.Processing;
        await Task.Delay(700, cancellationToken);
        job.Status = ExportJobStatus.Completed;
        job.CompletedAt = DateTimeOffset.UtcNow;
        job.ResultUrl = $"https://acc.autodesk.com/output/{job.Id}.ifc";
        return job;
    }
}
