using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Infrastructure.Aps;

public sealed class AccBrowserService : IAccBrowserService
{
    public Task<IReadOnlyList<AccNode>> GetHubsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AccNode>>(
        [
            new AccNode { Id = "hub-1", Name = "Enterprise Hub", Type = "hub" },
            new AccNode { Id = "hub-2", Name = "Regional Hub", Type = "hub" }
        ]);

    public Task<IReadOnlyList<AccNode>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        var nodes = new List<AccNode>();
        if (parentId.StartsWith("hub"))
        {
            nodes.Add(new AccNode { Id = parentId + "-project-1", Name = "Airport Expansion", Type = "project", ParentId = parentId });
            nodes.Add(new AccNode { Id = parentId + "-project-2", Name = "Hospital Tower", Type = "project", ParentId = parentId });
        }
        else if (parentId.Contains("project"))
        {
            nodes.Add(new AccNode { Id = parentId + "-folder-design", Name = "Design", Type = "folder", ParentId = parentId });
            nodes.Add(new AccNode { Id = parentId + "-folder-published", Name = "Published", Type = "folder", ParentId = parentId });
        }
        else if (parentId.Contains("folder"))
        {
            nodes.Add(new AccNode { Id = parentId + "-file-core", Name = "CoreModel.rvt", Type = "file", ParentId = parentId });
            nodes.Add(new AccNode { Id = parentId + "-file-shell", Name = "ShellModel.rvt", Type = "file", ParentId = parentId });
        }

        return Task.FromResult<IReadOnlyList<AccNode>>(nodes);
    }
}
