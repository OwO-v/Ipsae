namespace Ipsae.Model;

public class DetectionEvent
{
    public string ProcessName { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string DetectedDate { get; set; } = "";
    public string Severity { get; set; } = "";
}
