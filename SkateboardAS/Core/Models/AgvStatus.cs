namespace Core.Models;

public class AgvStatus
{
    public string Position { get; set; } = string.Empty;
    public double BatteryLevel { get; set; }
    public string CurrentProgram { get; set; } = string.Empty;
    public int State { get; set; }
}
