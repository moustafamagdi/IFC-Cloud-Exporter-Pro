using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface IAccBrowserService
{
    Task<IReadOnlyList<AccNode>> GetHubsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccNode>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default);
}
