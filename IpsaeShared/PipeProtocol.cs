namespace IpsaeShared;

public static class PipeProtocol
{
    public const string ClientPipeName = "IpsaeIDS";
    public const string EnginePipeName = "IpsaeEngine";
}

public enum PipeCommand : byte
{
    // Client -> Service
    QueryStatus = 0x01,
    StartService = 0x02,
    StopService = 0x03,

    // Engine -> Service
    ActiveEngine = 0x11,
    InactiveEngine = 0x12,
    StartingEngine = 0x13,
    StoppingEngine = 0x14,
    ErrorEngine = 0x15,

    // Service -> Client
    StatusResponse = 0x81,

}


public enum ServiceStatusCode : byte
{
    Active = 0,
    Inactive = 1,
    Starting = 2,
    Stopping = 3,
    Error = 4,
}
