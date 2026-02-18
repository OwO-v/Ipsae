using System.Windows;
using Ipsae.Service;

namespace Ipsae;

public partial class App : Application
{
    private static INavigationService? _navigationService;

    public static INavigationService NavigationService
    {
        get => _navigationService ?? throw new InvalidOperationException("NavigationService not initialized");
        set => _navigationService = value;
    }
}
