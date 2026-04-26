using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface IHistoryRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ExportHistoryRecord record, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExportHistoryRecord>> GetRecentAsync(int limit = 500, CancellationToken cancellationToken = default);
}
