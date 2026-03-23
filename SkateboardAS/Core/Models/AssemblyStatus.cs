namespace Core.Models;

public class AssemblyStatus
{
    public string CurrentOperation { get; set; } = string.Empty;
    public double Progress { get; set; }
    public bool IsHealthy { get; set; }
    public int State { get; set; }
}
