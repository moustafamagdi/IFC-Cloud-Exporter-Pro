using System.Threading.Channels;
using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Application.Services;

public sealed class BatchExportService : IExportOrchestrator
{
    private readonly Channel<ExportJob> _queue = Channel.CreateUnbounded<ExportJob>();

    public async Task QueueAsync(IEnumerable<ExportJob> jobs, CancellationToken cancellationToken = default)
    {
        foreach (var job in jobs)
        {
            await _queue.Writer.WriteAsync(job, cancellationToken);
        }
    }

    public async IAsyncEnumerable<ExportJob> ProcessQueueAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (await _queue.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_queue.Reader.TryRead(out var job))
            {
                yield return job;
            }
        }
    }
}
