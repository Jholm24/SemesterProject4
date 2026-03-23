namespace Orchestration.Registry;

public class VersionedComponentRegistry
{
    private readonly Dictionary<string, string> _registry = new();

    public void Register(string componentName, string version)
        => _registry[componentName] = version;

    public bool IsCompatible(string componentName, string requiredVersion)
    {
        // TODO: Implement - semantic versioning compatibility check
        if (!_registry.TryGetValue(componentName, out var registeredVersion))
            return false;
        return registeredVersion == requiredVersion;
    }
}
