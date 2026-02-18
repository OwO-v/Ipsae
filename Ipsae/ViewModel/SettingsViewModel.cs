using System.Windows.Input;
using Ipsae.Service;

namespace Ipsae.ViewModel;

public class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    private string _alarmType = "Type 1";
    private bool _runAlertEnabled;
    private bool _autoUpdateEnabled = true;
    private string _threatUpdateInterval = "1 Day";
    private string _interfaceName = "eno1";
    private string _maxDelay = "3 Sec";

    public SettingsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        NavigateHomeCommand = new RelayCommand(_ => _navigationService.NavigateHome());
        ExportCommand = new RelayCommand(_ => Export());
        ImportCommand = new RelayCommand(_ => Import());
    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ImportCommand { get; }

    public string AlarmType
    {
        get => _alarmType;
        set => SetProperty(ref _alarmType, value);
    }

    public bool RunAlertEnabled
    {
        get => _runAlertEnabled;
        set
        {
            if (SetProperty(ref _runAlertEnabled, value))
            {
                OnPropertyChanged(nameof(RunAlertDisplayText));
            }
        }
    }

    public string RunAlertDisplayText => RunAlertEnabled ? "On" : "Off";

    public bool AutoUpdateEnabled
    {
        get => _autoUpdateEnabled;
        set
        {
            if (SetProperty(ref _autoUpdateEnabled, value))
            {
                OnPropertyChanged(nameof(AutoUpdateDisplayText));
            }
        }
    }

    public string AutoUpdateDisplayText => AutoUpdateEnabled ? "On" : "Off";

    public string ThreatUpdateInterval
    {
        get => _threatUpdateInterval;
        set => SetProperty(ref _threatUpdateInterval, value);
    }

    public string InterfaceName
    {
        get => _interfaceName;
        set => SetProperty(ref _interfaceName, value);
    }

    public string MaxDelay
    {
        get => _maxDelay;
        set => SetProperty(ref _maxDelay, value);
    }

    private void Export()
    {
    }

    private void Import()
    {
    }
}
