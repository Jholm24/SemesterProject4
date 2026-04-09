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
        var exports = _container.GetExports<Lazy<IMachineComponent, IComponentMetadata>>();
        return exports.Select(e => (e.Value, e.Metadata));
    }

    public T? Resolve<T>() where T : class
        => _container.TryGetExport<T>(out var value) ? value : null;
}
