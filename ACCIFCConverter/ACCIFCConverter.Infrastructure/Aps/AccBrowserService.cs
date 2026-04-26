using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Infrastructure.Aps;

public sealed class AccBrowserService : IAccBrowserService
{
    public Task<IReadOnlyList<AccNode>> GetHubsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AccNode>>([new AccNode{Id="sample-hub",Name="Sample Hub",Type="hub"}]);

    public Task<IReadOnlyList<AccNode>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AccNode> children =
        [
            new AccNode{Id=$"{parentId}-project",Name="Project A",Type="project",ParentId=parentId},
            new AccNode{Id=$"{parentId}-folder",Name="Models",Type="folder",ParentId=parentId},
            new AccNode{Id=$"{parentId}-file",Name="BuildingA.rvt",Type="file",ParentId=parentId}
        ];
        return Task.FromResult(children);
    }
}
