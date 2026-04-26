using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface IExportOrchestrator
{
    Task QueueAsync(IEnumerable<ExportJob> jobs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ExportJob> ProcessQueueAsync(CancellationToken cancellationToken = default);
}
