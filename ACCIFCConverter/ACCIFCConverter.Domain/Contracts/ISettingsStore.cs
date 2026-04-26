using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface ISettingsStore
{
    Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
