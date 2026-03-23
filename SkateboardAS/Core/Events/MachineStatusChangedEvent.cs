using Core.Models;

namespace Core.Events;

public class MachineStatusChangedEvent
{
    public string MachineId { get; set; } = string.Empty;
    public MachineStatusModel Status { get; set; } = new();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
