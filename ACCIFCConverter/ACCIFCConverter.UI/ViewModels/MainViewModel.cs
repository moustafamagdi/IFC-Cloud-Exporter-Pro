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
    private readonly OutputNamingService _outputNaming;

    [ObservableProperty] private string _statusText = "Ready";
    [ObservableProperty] private string _selectedPreset = "IFC4 Reference";
    [ObservableProperty] private string _namingTemplate = "{Project}_{Model}_{Date}.ifc";
    [ObservableProperty] private string _outputPreview = string.Empty;
    [ObservableProperty] private AppSettings _settings = new();

    public ObservableCollection<string> Presets { get; } = ["IFC2x3 Coordination", "IFC4 Reference", "IFC4 Design Transfer", "COBie"];
    public ObservableCollection<AccNode> BrowserNodes { get; } = [];
    public ObservableCollection<ExportJob> Jobs { get; } = [];
    public ObservableCollection<ExportHistoryRecord> History { get; } = [];

    public MainViewModel(IApsAuthService authService, IAccBrowserService browserService, IHistoryRepository historyRepository, ISettingsStore settingsStore, OutputNamingService outputNaming)
    {
        _authService = authService;
        _browserService = browserService;
        _historyRepository = historyRepository;
        _settingsStore = settingsStore;
        _outputNaming = outputNaming;
        OutputPreview = _outputNaming.Render(NamingTemplate, "DemoProject", "BuildingA", "R1");
        _ = InitializeAsync();
    }

    partial void OnNamingTemplateChanged(string value) => OutputPreview = _outputNaming.Render(value, "DemoProject", "BuildingA", "R1");

    private async Task InitializeAsync()
    {
        await _historyRepository.InitializeAsync();
        Settings = await _settingsStore.LoadAsync();
        var hubs = await _browserService.GetHubsAsync();
        BrowserNodes.Clear();
        foreach (var hub in hubs) BrowserNodes.Add(hub);
        var history = await _historyRepository.GetRecentAsync(100);
        History.Clear();
        foreach (var item in history) History.Add(item);
    }

    [RelayCommand]
    private async Task Login()
    {
        StatusText = "Logging in...";
        var ok = await _authService.LoginAsync();
        StatusText = ok ? "Authenticated" : "Login failed";
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
        StatusText = "Logged out";
    }

    [RelayCommand]
    private async Task SaveSettings() => await _settingsStore.SaveAsync(Settings);

    [RelayCommand] private void ShowDashboard() { }
    [RelayCommand] private void ShowBrowser() { }
    [RelayCommand] private void ShowQueue() { }
    [RelayCommand] private void ShowHistory() { }
    [RelayCommand] private void ShowSettings() { }
    [RelayCommand] private void ShowAbout() { }
}
