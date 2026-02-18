using System.Windows.Input;
using Ipsae.Service;

namespace Ipsae.ViewModel;

public class ServiceStatusViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    private string _serviceStatusText = "잎새가 동작하고 있습니다.";
    private bool _isServiceRunning = true;
    private int _inspectedCount;
    private int _threatCount;
    private string _interfaceName = "eno1";
    private string _uptime = "00:00:00";
    private string _maxDelay = "3 sec";
    private string _logCount = "0 lines";
    private string _footerPackets = "0 packets";
    private bool _isStatusTabActive = true;

    public ServiceStatusViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        NavigateHomeCommand = new RelayCommand(_ => _navigationService.NavigateHome());
        ToggleServiceCommand = new RelayCommand(_ => ToggleService());
        SwitchToStatusTabCommand = new RelayCommand(_ => IsStatusTabActive = true);
        SwitchToStatsTabCommand = new RelayCommand(_ => IsStatusTabActive = false);
    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand ToggleServiceCommand { get; }
    public ICommand SwitchToStatusTabCommand { get; }
    public ICommand SwitchToStatsTabCommand { get; }

    public string ServiceStatusText
    {
        get => _serviceStatusText;
        set => SetProperty(ref _serviceStatusText, value);
    }

    public bool IsServiceRunning
    {
        get => _isServiceRunning;
        set => SetProperty(ref _isServiceRunning, value);
    }

    public int InspectedCount
    {
        get => _inspectedCount;
        set => SetProperty(ref _inspectedCount, value);
    }

    public int ThreatCount
    {
        get => _threatCount;
        set => SetProperty(ref _threatCount, value);
    }

    public string InterfaceName
    {
        get => _interfaceName;
        set => SetProperty(ref _interfaceName, value);
    }

    public string Uptime
    {
        get => _uptime;
        set => SetProperty(ref _uptime, value);
    }

    public string MaxDelay
    {
        get => _maxDelay;
        set => SetProperty(ref _maxDelay, value);
    }

    public string LogCount
    {
        get => _logCount;
        set => SetProperty(ref _logCount, value);
    }

    public string FooterPackets
    {
        get => _footerPackets;
        set => SetProperty(ref _footerPackets, value);
    }

    public bool IsStatusTabActive
    {
        get => _isStatusTabActive;
        set => SetProperty(ref _isStatusTabActive, value);
    }

    private void ToggleService()
    {
    }
}
