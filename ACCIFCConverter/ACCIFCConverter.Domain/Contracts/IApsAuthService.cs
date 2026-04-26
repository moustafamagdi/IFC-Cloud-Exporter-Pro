using ACCIFCConverter.Domain.Models;

namespace ACCIFCConverter.Domain.Contracts;

public interface IApsAuthService
{
    Task<bool> LoginAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    bool IsAuthenticated { get; }
}
