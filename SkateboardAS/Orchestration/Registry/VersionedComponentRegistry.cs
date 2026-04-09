namespace Orchestration.Registry;

public class VersionedComponentRegistry
{
    private readonly Dictionary<string, string> _registry = new();

    public void Register(string componentName, string version)
        => _registry[componentName] = version;

    public bool IsCompatible(string componentName, string requiredVersion)
    {
        if (!_registry.TryGetValue(componentName, out var registeredVersionStr))
            return false;

        if (!Version.TryParse(registeredVersionStr, out var registered))
            return false;

        if (!Version.TryParse(requiredVersion, out var required))
            return false;

        // Major versions must match exactly
        if (registered.Major != required.Major)
            return false;

        // Registered minor must be >= required minor
        if (registered.Minor < required.Minor)
            return false;

        // If minor versions match, registered patch must be >= required patch
        if (registered.Minor == required.Minor && registered.Build < required.Build)
            return false;

        return true;
    }
}
