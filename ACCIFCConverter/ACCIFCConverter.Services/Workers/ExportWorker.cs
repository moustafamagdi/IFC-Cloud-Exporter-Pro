using ACCIFCConverter.Domain.Contracts;

namespace ACCIFCConverter.Services.Workers;

public sealed class ExportWorker(IExportOrchestrator orchestrator)
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var _ in orchestrator.ProcessQueueAsync(cancellationToken))
        {
            // Hook for future hosted background execution.
        }
    }
}
