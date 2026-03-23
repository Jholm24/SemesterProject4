namespace Core.Models;

public class ProductionStatus
{
    public string ProductionLineId { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public bool IsRunning { get; set; }
    public bool IsHealthy { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
