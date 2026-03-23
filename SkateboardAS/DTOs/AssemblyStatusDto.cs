namespace SkateboardAS.DTOs;

public class AssemblyStatusDto
{
    public string Id { get; set; } = string.Empty;
    public string CurrentOperation { get; set; } = string.Empty;
    public double Progress { get; set; }
    public bool IsHealthy { get; set; }
    public string State { get; set; } = string.Empty;
}
