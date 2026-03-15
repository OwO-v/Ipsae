using System.IO;
using System.Windows;
using Ipsae.Ipc;
using Ipsae.ViewModel;
using IpsaeShared;
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

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Logger
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(IpsaePaths.LogDir, "ipsae-ui.log"),
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 5 * 1024 * 1024,
                retainedFileCountLimit: 3)
            .CreateLogger();

        Log.Information("Ipsae UI starting");

        // 2. Service connection check
        var status = await IpcClient.Instance.QueryStatusAsync();
        if (status == null)
        {
            Log.Error("Service connection failed");
            MessageBox.Show("IpsaeIDS 서비스에 연결할 수 없습니다.", "연결 오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            Log.Information("Service connected, status: {Status}", status);
            ServiceState.Instance.Status = status.Value;
        }

        // 3. Database check & create
        if (!DatabaseService.Instance.Initialize())
        {
            MessageBox.Show("데이터베이스 초기화에 실패했습니다.", "DB 오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Ipsae UI exiting");
        Log.CloseAndFlush();

        base.OnExit(e);
    }
}
