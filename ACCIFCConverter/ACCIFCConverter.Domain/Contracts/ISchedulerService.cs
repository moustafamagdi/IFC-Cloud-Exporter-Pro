namespace ACCIFCConverter.Domain.Contracts;

public interface ISchedulerService
{
    Task ScheduleAsync(string jobName, string executablePath, string arguments, string cadence, CancellationToken cancellationToken = default);
}
