using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Ipsae.Service;

public enum ServiceStatus
{
    Active,
    Inactive,
    Starting,
    Stopping
}

public class ServiceState : INotifyPropertyChanged
{
    private static readonly ServiceState _instance = new();
    public static ServiceState Instance => _instance;

    private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(0x44, 0xAA, 0x55));
    private static readonly SolidColorBrush RedBrush = new(Color.FromRgb(0xCC, 0x55, 0x55));
    private static readonly SolidColorBrush OrangeBrush = new(Color.FromRgb(0xCC, 0x88, 0x33));
    private static readonly Color GreenGlow = Color.FromRgb(0x44, 0xAA, 0x55);
    private static readonly Color RedGlow = Color.FromRgb(0xCC, 0x55, 0x55);
    private static readonly Color OrangeGlow = Color.FromRgb(0xCC, 0x88, 0x33);

    private ServiceStatus _status = ServiceStatus.Active;

    private ServiceState() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ServiceStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(IsTransitioning));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusSub));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(StatusGlowColor));
            OnPropertyChanged(nameof(StatusLabel));
        }
    }

    public bool IsRunning => _status == ServiceStatus.Active;
    public bool IsTransitioning => _status is ServiceStatus.Starting or ServiceStatus.Stopping;

    public string StatusText => _status switch
    {
        ServiceStatus.Active => "Active",
        ServiceStatus.Inactive => "Inactive",
        ServiceStatus.Starting => "Starting",
        ServiceStatus.Stopping => "Stopping",
        _ => "Unknown"
    };

    public string StatusSub => _status switch
    {
        ServiceStatus.Active => "동작중",
        ServiceStatus.Inactive => "동작 중지",
        ServiceStatus.Starting => "시작 중",
        ServiceStatus.Stopping => "중지 중",
        _ => "알 수 없음"
    };

    public SolidColorBrush StatusColor => _status switch
    {
        ServiceStatus.Active => GreenBrush,
        ServiceStatus.Inactive => RedBrush,
        ServiceStatus.Starting or ServiceStatus.Stopping => OrangeBrush,
        _ => RedBrush
    };

    public Color StatusGlowColor => _status switch
    {
        ServiceStatus.Active => GreenGlow,
        ServiceStatus.Inactive => RedGlow,
        ServiceStatus.Starting or ServiceStatus.Stopping => OrangeGlow,
        _ => RedGlow
    };

    public string StatusLabel => _status switch
    {
        ServiceStatus.Active => "● MONITORING",
        ServiceStatus.Inactive => "● INACTIVE",
        ServiceStatus.Starting => "● STARTING",
        ServiceStatus.Stopping => "● STOPPING",
        _ => "● UNKNOWN"
    };

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
