using System.Windows;
using Ipsae.ViewModel;
using Serilog;

namespace Ipsae;

public partial class App : Application
{
    private static INavigationService? _navigationService;

    public static INavigationService NavigationService
    {
        get => _navigationService ?? throw new InvalidOperationException("NavigationService not initialized");
        set => _navigationService = value;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/ipsae-ui.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 5 * 1024 * 1024,
                retainedFileCountLimit: 3)
            .CreateLogger();

        Log.Information("Ipsae UI starting");

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Ipsae UI exiting");
        Log.CloseAndFlush();

        base.OnExit(e);
    }
}
