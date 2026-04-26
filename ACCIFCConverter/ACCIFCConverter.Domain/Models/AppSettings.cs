namespace ACCIFCConverter.Domain.Models;

public sealed class AppSettings
{
    public string ApsClientId { get; set; } = string.Empty;
    public string ApsClientSecret { get; set; } = string.Empty;
    public string ApsCallbackUrl { get; set; } = "http://localhost:5005/callback";
    public string DefaultOutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string Theme { get; set; } = "Dark";
    public string Language { get; set; } = "en-US";
    public int RetryCount { get; set; } = 3;
}
