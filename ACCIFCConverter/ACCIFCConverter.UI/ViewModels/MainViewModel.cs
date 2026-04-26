using System.Collections.ObjectModel;
using ACCIFCConverter.Application.Services;
using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACCIFCConverter.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IApsAuthService _authService;
    private readonly IAccBrowserService _browserService;
    private readonly IHistoryRepository _historyRepository;
    private readonly ISettingsStore _settingsStore;
    private readonly IExportOrchestrator _exportOrchestrator;
    private readonly ISchedulerService _schedulerService;
    private readonly OutputNamingService _outputNaming;

    [ObservableProperty] private string _statusText = "Ready";
    [ObservableProperty] private string _selectedPreset = "IFC4 Reference";
    [ObservableProperty] private string _namingTemplate = "{Project}_{Model}_{Date}.ifc";
    [ObservableProperty] private string _outputPreview = string.Empty;
    [ObservableProperty] private AppSettings _settings = new();
    [ObservableProperty] private string _selectedCadence = "daily";
    [ObservableProperty] private string _searchTerm = string.Empty;

    public ObservableCollection<string> Presets { get; } = ["IFC2x3 Coordination", "IFC4 Reference", "IFC4 Design Transfer", "COBie"];
    public ObservableCollection<string> Cadences { get; } = ["daily", "weekly", "monthly"];
    public ObservableCollection<AccNode> BrowserNodes { get; } = [];
    public ObservableCollection<ExportJob> Jobs { get; } = [];
    public ObservableCollection<ExportHistoryRecord> History { get; } = [];
    public ObservableCollection<ScheduledJob> ScheduledJobs { get; } = [];

    public MainViewModel(
        IApsAuthService authService,
        IAccBrowserService browserService,
        IHistoryRepository historyRepository,
        ISettingsStore settingsStore,
        IExportOrchestrator exportOrchestrator,
        ISchedulerService schedulerService,
        OutputNamingService outputNaming)
    {
        _authService = authService;
        _browserService = browserService;
        _historyRepository = historyRepository;
        _settingsStore = settingsStore;
        _exportOrchestrator = exportOrchestrator;
        _schedulerService = schedulerService;
        _outputNaming = outputNaming;
        OutputPreview = _outputNaming.Render(NamingTemplate, "DemoProject", "BuildingA", "R1");
        _ = InitializeAsync();
    }

    partial void OnNamingTemplateChanged(string value) => OutputPreview = _outputNaming.Render(value, "DemoProject", "BuildingA", "R1");

    private async Task InitializeAsync()
    {
        await _historyRepository.InitializeAsync();
        Settings = await _settingsStore.LoadAsync();
        await LoadBrowserAsync();
        await LoadHistoryAsync();
    }

    [RelayCommand]
    private async Task Login()
    {
        StatusText = "Logging in...";
        var ok = await _authService.LoginAsync();
        StatusText = ok ? "Authenticated to ACC" : "Login failed";
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
        StatusText = "Logged out";
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        await _settingsStore.SaveAsync(Settings);
        StatusText = "Settings saved";
    }

    [RelayCommand]
    private async Task LoadBrowserAsync()
    {
        BrowserNodes.Clear();
        var hubs = await _browserService.GetHubsAsync();
        foreach (var hub in hubs)
        {
            BrowserNodes.Add(hub);
            var projects = await _browserService.GetChildrenAsync(hub.Id);
            foreach (var project in projects)
            {
                BrowserNodes.Add(project);
                var folders = await _browserService.GetChildrenAsync(project.Id);
                foreach (var folder in folders)
                {
                    BrowserNodes.Add(folder);
                    var files = await _browserService.GetChildrenAsync(folder.Id);
                    foreach (var file in files.Where(f => f.Name.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (string.IsNullOrWhiteSpace(SearchTerm) || file.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            BrowserNodes.Add(file);
                        }
                    }
                }
            }
        }
        StatusText = $"Loaded {BrowserNodes.Count} ACC items";
    }

    [RelayCommand]
    private async Task AddSampleJob()
    {
        var file = BrowserNodes.FirstOrDefault(x => x.Type == "file") ?? new AccNode { Id = "local-demo-file", Name = "DemoModel.rvt", Type = "file" };
        var job = new ExportJob
        {
            SourceFileId = file.Id,
            SourceFileName = file.Name,
            ProjectId = "project-1",
            HubId = "hub-1",
            DestinationFolderId = "project-1-folder-published",
            Preset = SelectedPreset,
            OutputFileName = OutputPreview
        };

        Jobs.Add(job);
        await _exportOrchestrator.QueueAsync([job]);
        StatusText = "Job queued";
    }

    [RelayCommand]
    private async Task ProcessQueue()
    {
        StatusText = "Processing queue";
        await foreach (var updated in _exportOrchestrator.ProcessQueueAsync())
        {
            var existing = Jobs.FirstOrDefault(x => x.Id == updated.Id);
            if (existing is not null)
            {
                var idx = Jobs.IndexOf(existing);
                Jobs[idx] = updated;
            }
        }

        await LoadHistoryAsync();
        StatusText = "Queue complete";
    }

    [RelayCommand]
    private async Task ScheduleExport()
    {
        var jobName = $"ACCIFC_{DateTime.Now:yyyyMMdd_HHmmss}";
        await _schedulerService.ScheduleAsync(jobName, "ACCIFCConverter.UI.exe", "--run-scheduled", SelectedCadence);
        ScheduledJobs.Add(new ScheduledJob { Name = jobName, Cadence = SelectedCadence, Arguments = "--run-scheduled" });
        StatusText = $"Scheduled {SelectedCadence} export";
    }

    private async Task LoadHistoryAsync()
    {
        var history = await _historyRepository.GetRecentAsync(100);
        History.Clear();
        foreach (var item in history) History.Add(item);
    }

    [RelayCommand] private void ShowDashboard() { }
    [RelayCommand] private void ShowBrowser() { }
    [RelayCommand] private void ShowQueue() { }
    [RelayCommand] private void ShowHistory() { }
    [RelayCommand] private void ShowSettings() { }
    [RelayCommand] private void ShowAbout() { }
}
