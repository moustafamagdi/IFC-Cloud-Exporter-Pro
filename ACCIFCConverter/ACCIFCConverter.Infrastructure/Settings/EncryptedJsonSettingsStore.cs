using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;
using ACCIFCConverter.Infrastructure.Security;
using Newtonsoft.Json;

namespace ACCIFCConverter.Infrastructure.Settings;

public sealed class EncryptedJsonSettingsStore : ISettingsStore
{
    private readonly string _filePath;
    private readonly DpapiSecretProtector _protector = new();

    public EncryptedJsonSettingsStore(string filePath) => _filePath = filePath;

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath)) return new AppSettings();
        var json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        var encrypted = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
        return new AppSettings
        {
            ApsClientId = encrypted.TryGetValue("ApsClientId", out var id) ? _protector.Unprotect(id) : string.Empty,
            ApsClientSecret = encrypted.TryGetValue("ApsClientSecret", out var sec) ? _protector.Unprotect(sec) : string.Empty,
            ApsCallbackUrl = encrypted.GetValueOrDefault("ApsCallbackUrl", "http://localhost:5005/callback"),
            DefaultOutputFolder = encrypted.GetValueOrDefault("DefaultOutputFolder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
            Theme = encrypted.GetValueOrDefault("Theme", "Dark"),
            Language = encrypted.GetValueOrDefault("Language", "en-US"),
            RetryCount = int.TryParse(encrypted.GetValueOrDefault("RetryCount", "3"), out var val) ? val : 3
        };
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["ApsClientId"] = _protector.Protect(settings.ApsClientId),
            ["ApsClientSecret"] = _protector.Protect(settings.ApsClientSecret),
            ["ApsCallbackUrl"] = settings.ApsCallbackUrl,
            ["DefaultOutputFolder"] = settings.DefaultOutputFolder,
            ["Theme"] = settings.Theme,
            ["Language"] = settings.Language,
            ["RetryCount"] = settings.RetryCount.ToString()
        };
        await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(payload, Formatting.Indented), cancellationToken);
    }
}
