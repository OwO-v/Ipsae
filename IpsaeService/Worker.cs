using System.Diagnostics;
using System.IO.Pipes;
using IpsaeShared;

namespace IpsaeService;

public enum EngineAction
{
    None,
    Start,
    Stop,
}

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly object _lock = new();

    private volatile ServiceStatusCode _status = ServiceStatusCode.Inactive;
    private volatile EngineAction _requestedAction = EngineAction.Start;

    private readonly string EnginePath = Path.Combine(AppContext.BaseDirectory, "IpsaeEngine.exe");
    private const int EngineCheckIntervalMs = 2000;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("IpsaeIDS Service starting");
        await Task.Yield();

        var engineTask = EngineManagerLoop(stoppingToken);
        var pipeTask = PipeServerLoop(stoppingToken);

        await Task.WhenAll(engineTask, pipeTask);

        _logger.LogInformation("IpsaeIDS Service stopped");
    }

    private async Task EngineManagerLoop(CancellationToken ct)
    {
        Process? engineProcess = null;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                EngineAction action;

                lock (_lock)
                {
                    action = _requestedAction;
                    _requestedAction = EngineAction.None;
                }

                switch (action)
                {
                    case EngineAction.Start when _status is ServiceStatusCode.Inactive or ServiceStatusCode.Stopping:
                        engineProcess = StartEngine();
                        break;

                    case EngineAction.Stop when _status is ServiceStatusCode.Active or ServiceStatusCode.Starting:
                        StopEngine(engineProcess);
                        engineProcess = null;
                        break;
                }

                if (_status == ServiceStatusCode.Active && engineProcess != null && engineProcess.HasExited)
                {
                    _logger.LogWarning("Engine process exited unexpectedly (exit code: {ExitCode})", engineProcess.ExitCode);
                    engineProcess.Dispose();
                    engineProcess = null;
                    lock (_lock)
                    {
                        _status = ServiceStatusCode.Inactive;
                    }
                }

                await Task.Delay(EngineCheckIntervalMs, ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        finally
        {
            StopEngine(engineProcess);
        }
    }

    private Process? StartEngine()
    {
        try
        {
            lock (_lock)
            {
                _status = ServiceStatusCode.Starting;
            }

            _logger.LogInformation("Starting engine: {Path}", EnginePath);

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = EnginePath,
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            if (process == null || process.HasExited)
            {
                _logger.LogError("Failed to start engine process");
                lock (_lock)
                {
                    _status = ServiceStatusCode.Inactive;
                }
                return null;
            }

            lock (_lock)
            {
                _status = ServiceStatusCode.Active;
            }

            _logger.LogInformation("Engine started (PID: {Pid})", process.Id);
            return process;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start engine");
            lock (_lock)
            {
                _status = ServiceStatusCode.Inactive;
            }
            return null;
        }
    }

    private void StopEngine(Process? process)
    {
        if (process == null || process.HasExited)
        {
            lock (_lock)
            {
                _status = ServiceStatusCode.Inactive;
            }
            return;
        }

        try
        {
            lock (_lock)
            {
                _status = ServiceStatusCode.Stopping;
            }

            _logger.LogInformation("Stopping engine (PID: {Pid})", process.Id);

            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }

            process.Dispose();

            lock (_lock)
            {
                _status = ServiceStatusCode.Inactive;
            }

            _logger.LogInformation("Engine stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping engine");
            lock (_lock)
            {
                _status = ServiceStatusCode.Inactive;
            }
        }
    }

    private async Task PipeServerLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    PipeProtocol.PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                await server.WaitForConnectionAsync(ct);
                _logger.LogInformation("Client connected");

                await HandleClientAsync(server, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pipe error");
            }
        }
    }

    private async Task HandleClientAsync(NamedPipeServerStream server, CancellationToken ct)
    {
        try
        {
            while (server.IsConnected && !ct.IsCancellationRequested)
            {
                var message = await PipeMessage.ReadAsync(server, ct);
                if (message == null) break;

                var response = HandleMessage(message);
                if (response != null)
                {
                    var bytes = response.ToBytes();
                    await server.WriteAsync(bytes, ct);
                    await server.FlushAsync(ct);
                }
            }
        }
        catch (IOException)
        {
            _logger.LogInformation("Client disconnected");
        }
    }

    private PipeMessage? HandleMessage(PipeMessage message)
    {
        switch (message.Command)
        {
            case PipeCommand.QueryStatus:
                return PipeMessage.Status(_status);

            case PipeCommand.StartService:
                _logger.LogInformation("Start command received");
                lock (_lock)
                {
                    _requestedAction = EngineAction.Start;
                    _status = ServiceStatusCode.Starting;
                }
                return PipeMessage.Status(_status);

            case PipeCommand.StopService:
                _logger.LogInformation("Stop command received");
                lock (_lock)
                {
                    _requestedAction = EngineAction.Stop;
                    _status = ServiceStatusCode.Stopping;
                }
                return PipeMessage.Status(_status);

            default:
                return null;
        }
    }
}
