using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ipsae.Service;

public class ServiceState : INotifyPropertyChanged
{
    private static readonly ServiceState _instance = new();
    public static ServiceState Instance => _instance;

    private bool _isRunning = true;

    private ServiceState() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
        }
    }

    public string StatusText => IsRunning ? "Active" : "Inactive";

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
