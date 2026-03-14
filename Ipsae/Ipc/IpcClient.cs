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
            // 클라이언트 스트림을 생성, 타임아웃이 발생하면 예외가 발생하도록 cts 토큰 설정
            await using var client = new NamedPipeClientStream(".", PipeProtocol.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            using var cts = new CancellationTokenSource(timeoutMs);

            // 클라이언트 연결을 비동기로 시도하고, 타임아웃이 발생하면 예외가 발생하도록 설정
            await client.ConnectAsync(cts.Token);

            // 명령을 PipeMessage로 변환하여 바이트 배열로 직렬화한 후, 파이프에 쓰고 플러시
            var request = PipeMessage.FromCommand(command);
            var bytes = request.ToBytes();
            await client.WriteAsync(bytes, cts.Token);
            await client.FlushAsync(cts.Token);

            // 서버로부터 응답을 비동기로 읽고, 타임아웃이 발생하면 예외가 발생하도록 설정
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

    /// <summary>
    /// 서비스 상태 조회 명령을 보내고, 서버로부터 응답을 받아 서비스 상태 코드를 반환합니다.
    /// </summary>
    /// <returns>성공 시 ServiceStatusCode 반환, 오류 발생 시 Null 반환</returns>
    public async Task<ServiceStatusCode?> QueryStatusAsync()
    {
        return await SendCommandAsync(PipeCommand.QueryStatus);
    }

    /// <summary>
    /// 서비스 시작 명령을 보내고, 서버로부터 응답을 받아 서비스 상태 코드를 반환합니다.
    /// </summary>
    /// <returns>성공 시 ServiceStatusCode 반환, 오류 발생 시 Null 반환</returns>
    public async Task<ServiceStatusCode?> StartServiceAsync()
    {
        return await SendCommandAsync(PipeCommand.StartService);
    }

    /// <summary>
    /// 서비스 종료 명령을 보내고, 서버로부터 응답을 받아 서비스 상태 코드를 반환합니다.
    /// </summary>
    /// <returns>성공 시 ServiceStatusCode 반환, 오류 발생 시 Null 반환</returns>
    public async Task<ServiceStatusCode?> StopServiceAsync()
    {
        return await SendCommandAsync(PipeCommand.StopService);
    }
}
