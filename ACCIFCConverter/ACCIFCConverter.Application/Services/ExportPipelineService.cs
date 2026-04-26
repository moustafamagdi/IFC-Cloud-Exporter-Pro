using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;
using Polly;
using Polly.Retry;

namespace ACCIFCConverter.Application.Services;

public sealed class ExportPipelineService(
    IExportExecutionService exportExecutionService,
    IFileTransferService fileTransferService,
    IHistoryRepository historyRepository) : IExportOrchestrator
{
    private readonly Queue<(ExportJob Job, ExportOptions Options)> _jobs = new();
    private readonly AsyncRetryPolicy _retry = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i));

    public Task QueueAsync(IEnumerable<ExportJob> jobs, CancellationToken cancellationToken = default)
    {
        foreach (var job in jobs)
        {
            _jobs.Enqueue((job, new ExportOptions { Preset = job.Preset ?? "IFC4 Reference", JsonConfigPath = job.JsonConfigPath }));
        }
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<ExportJob> ProcessQueueAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_jobs.Count > 0)
        {
            var (job, options) = _jobs.Dequeue();
            var started = DateTimeOffset.UtcNow;

            try
            {
                await _retry.ExecuteAsync(async token =>
                {
                    var downloadPath = await fileTransferService.DownloadAsync(job.SourceFileId, Path.GetTempPath(), token);
                    job.Status = ExportJobStatus.Uploading;
                    job.MappingFilePath = downloadPath;
                    await exportExecutionService.ExecuteAsync(job, options, token);

                    if (!string.IsNullOrWhiteSpace(job.ResultUrl))
                    {
                        await fileTransferService.UploadToAccAsync(job.ResultUrl, job.DestinationFolderId, token);
                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                job.Status = ExportJobStatus.Failed;
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTimeOffset.UtcNow;
            }

            if (job.Status is ExportJobStatus.Completed or ExportJobStatus.Failed)
            {
                await historyRepository.AddAsync(new ExportHistoryRecord
                {
                    JobId = job.Id,
                    FileName = job.SourceFileName,
                    StartUtc = started,
                    EndUtc = job.CompletedAt ?? DateTimeOffset.UtcNow,
                    DurationSeconds = (long)((job.CompletedAt ?? DateTimeOffset.UtcNow) - started).TotalSeconds,
                    Result = job.Status.ToString(),
                    OutputPath = job.ResultUrl ?? string.Empty,
                    Logs = job.ErrorMessage ?? string.Empty
                }, cancellationToken);
            }

            yield return job;
        }
    }
}
