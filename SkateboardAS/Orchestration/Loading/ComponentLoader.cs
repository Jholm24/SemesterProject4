using System.Reflection;
using System.Runtime.Loader;

namespace Orchestration.Loading;

public class ComponentLoader
{
    private readonly List<AssemblyLoadContext> _contexts = new();

    public Assembly LoadComponent(string dllPath)
    {
        var context = new ComponentLoadContext(dllPath);
        _contexts.Add(context);
        return context.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
    }

    public IEnumerable<Assembly> LoadedAssemblies =>
        _contexts.Select(c => c.Assemblies.FirstOrDefault()).Where(a => a != null)!;
}

internal class ComponentLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public ComponentLoadContext(string pluginPath) : base(isCollectible: true)
        => _resolver = new AssemblyDependencyResolver(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path != null ? LoadFromAssemblyPath(path) : null;
    }
}
