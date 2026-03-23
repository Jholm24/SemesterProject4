using System.Composition.Hosting;
using Core.Interfaces;
using Core.Metadata;

namespace Orchestration.Registry;

public class ComponentServiceRegistry
{
    private readonly CompositionHost _container;

    public ComponentServiceRegistry(CompositionHost container)
        => _container = container;

    public IEnumerable<(IMachineComponent Component, IComponentMetadata Metadata)> GetAll()
    {
        // TODO: Implement - use Lazy<T, TMetadata> for deferred instantiation
        return Enumerable.Empty<(IMachineComponent, IComponentMetadata)>();
    }

    public T? Resolve<T>() where T : class
        => _container.TryGetExport<T>(out var value) ? value : null;
}
