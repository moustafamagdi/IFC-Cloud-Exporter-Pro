using ACCIFCConverter.Domain.Contracts;

namespace ACCIFCConverter.Infrastructure.Scheduling;

public sealed class WindowsTaskSchedulerService : ISchedulerService
{
    public async Task ScheduleAsync(string jobName, string executablePath, string arguments, string cadence, CancellationToken cancellationToken = default)
    {
        var scheduleArg = cadence.ToLowerInvariant() switch
        {
            "daily" => "/sc daily",
            "weekly" => "/sc weekly",
            "monthly" => "/sc monthly",
            _ => throw new ArgumentOutOfRangeException(nameof(cadence), "Supported values: daily|weekly|monthly")
        };

        var psi = new System.Diagnostics.ProcessStartInfo("schtasks", $"/Create /F /TN "{jobName}" /TR "\"{executablePath}\" {arguments}" {scheduleArg} /ST 01:00")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = System.Diagnostics.Process.Start(psi) ?? throw new InvalidOperationException("Unable to start schtasks");
        await proc.WaitForExitAsync(cancellationToken);
        if (proc.ExitCode != 0) throw new InvalidOperationException($"schtasks failed with exit code {proc.ExitCode}");
    }
}
