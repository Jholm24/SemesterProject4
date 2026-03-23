namespace Shared.Entities;

public class TaskAssignment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OperatorId { get; set; } = string.Empty;
    public string ProductionLineId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
