using System.IO;
using System.IO.Pipes;
using IpsaeShared;

namespace Ipsae.Ipc;

public class IpcClient
{
    private static readonly Lazy<IpcClient> _instance = new(() => new IpcClient());
    public static IpcClient Instance => _instance.Value;

    private IpcClient() { }

    public async Task<ServiceStatusCode?> SendCommandAsync(PipeCommand command, int timeoutMs = 3000)
    {
        try
        {
            await using var client = new NamedPipeClientStream(".", PipeProtocol.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            using var cts = new CancellationTokenSource(timeoutMs);

            await client.ConnectAsync(cts.Token);

            var request = PipeMessage.FromCommand(command);
            var bytes = request.ToBytes();
            await client.WriteAsync(bytes, cts.Token);
            await client.FlushAsync(cts.Token);

            var response = await PipeMessage.ReadAsync(client, cts.Token);
            if (response?.Command == PipeCommand.StatusResponse)
            {
                return response.GetStatusCode();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ServiceStatusCode?> QueryStatusAsync()
    {
        return await SendCommandAsync(PipeCommand.QueryStatus);
    }

    public async Task<ServiceStatusCode?> StartServiceAsync()
    {
        return await SendCommandAsync(PipeCommand.StartService);
    }

    public async Task<ServiceStatusCode?> StopServiceAsync()
    {
        return await SendCommandAsync(PipeCommand.StopService);
    }
}
