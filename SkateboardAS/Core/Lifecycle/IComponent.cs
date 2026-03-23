namespace Core.Lifecycle;

public interface IComponent
{
    ComponentState State { get; }
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
}
