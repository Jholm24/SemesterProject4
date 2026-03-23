namespace SkateboardAS.DTOs;

public class AgvStatusDto
{
    public string Id { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public double BatteryLevel { get; set; }
    public string CurrentProgram { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}
