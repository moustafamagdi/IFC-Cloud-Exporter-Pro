using System.IO;
using System.Windows;
using ACCIFCConverter.Application.Services;
using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Infrastructure.Aps;
using ACCIFCConverter.Infrastructure.Persistence;
using ACCIFCConverter.Infrastructure.Scheduling;
using ACCIFCConverter.Infrastructure.Settings;
using ACCIFCConverter.Infrastructure.Storage;
using ACCIFCConverter.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ACCIFCConverter.UI;

public partial class App : System.Windows.Application
{
    public IServiceProvider Services { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(logDir);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(Path.Combine(logDir, "log-.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var sc = new ServiceCollection();
        sc.AddSingleton(new HttpClient());
        sc.AddSingleton<OutputNamingService>();
        sc.AddSingleton<IAccBrowserService, AccBrowserService>();
        sc.AddSingleton<IFileTransferService, LocalFileTransferService>();
        sc.AddSingleton<IExportExecutionService, DesignAutomationExportService>();
        sc.AddSingleton<IExportOrchestrator, ExportPipelineService>();
        sc.AddSingleton<IHistoryRepository>(_ => new SqliteHistoryRepository("Data Source=app.db"));
        sc.AddSingleton<ISettingsStore>(_ => new EncryptedJsonSettingsStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "secure.settings.json")));
        sc.AddSingleton<ISchedulerService, WindowsTaskSchedulerService>();
        sc.AddSingleton<IApsAuthService>(sp => new ApsAuthService(sp.GetRequiredService<HttpClient>(), "", "", "http://localhost:5005/callback"));
        sc.AddSingleton<MainViewModel>();

        Services = sc.BuildServiceProvider();
        base.OnStartup(e);

        var mainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainViewModel>() };
        mainWindow.Show();
    }
}
