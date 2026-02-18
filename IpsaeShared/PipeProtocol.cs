namespace IpsaeShared;

public static class PipeProtocol
{
    public const string PipeName = "IpsaeIDS";
}

public enum PipeCommand : byte
{
    // Client -> Service
    QueryStatus = 0x01,
    StartService = 0x02,
    StopService = 0x03,

    // Service -> Client
    StatusResponse = 0x81,
}

public enum ServiceStatusCode : byte
{
    Active = 0,
    Inactive = 1,
    Starting = 2,
    Stopping = 3,
}
