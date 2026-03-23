namespace Core.Events;

public class ComponentLoadedEvent
{
    public string ComponentName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime LoadedAt { get; set; } = DateTime.UtcNow;
}
