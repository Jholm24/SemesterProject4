using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;

namespace Orchestration.Loading;

public class MefCompositionBuilder
{
    private readonly List<Assembly> _assemblies = new();

    public MefCompositionBuilder AddAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
        return this;
    }

    public CompositionHost Build()
    {
        var configuration = new ContainerConfiguration()
            .WithAssemblies(_assemblies);
        return configuration.CreateContainer();
    }
}
