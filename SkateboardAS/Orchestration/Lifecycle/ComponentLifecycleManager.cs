using Core.Lifecycle;
using Microsoft.Extensions.Hosting;

namespace Orchestration.Lifecycle;

public class ComponentLifecycleManager : IHostedService
{
    private readonly IEnumerable<IComponent> _components;

    public ComponentLifecycleManager(IEnumerable<IComponent> components)
        => _components = components;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var component in _components)
        {
            await component.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var component in _components.Reverse())
        {
            await component.StopAsync(cancellationToken);
        }
    }
}
