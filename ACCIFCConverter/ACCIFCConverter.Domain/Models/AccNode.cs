namespace ACCIFCConverter.Domain.Models;

public sealed class AccNode
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? ParentId { get; init; }
    public bool IsFolder => Type is "hub" or "project" or "folder";
}
