using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface IExportExecutionService
{
    Task<ExportJob> ExecuteAsync(ExportJob job, ExportOptions options, CancellationToken cancellationToken = default);
}
