namespace ACCIFCConverter.Domain.Models;

public sealed class ScheduledJob
{
    public string Name { get; set; } = string.Empty;
    public string Cadence { get; set; } = "daily";
    public string Arguments { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
