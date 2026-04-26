namespace ACCIFCConverter.Application.Services;

public sealed class OutputNamingService
{
    public string Render(string template, string project, string model, string revision)
    {
        return template
            .Replace("{Project}", project, StringComparison.OrdinalIgnoreCase)
            .Replace("{Model}", model, StringComparison.OrdinalIgnoreCase)
            .Replace("{FileName}", model, StringComparison.OrdinalIgnoreCase)
            .Replace("{Revision}", revision, StringComparison.OrdinalIgnoreCase)
            .Replace("{Date}", DateTime.Now.ToString("yyyyMMdd"), StringComparison.OrdinalIgnoreCase);
    }
}
