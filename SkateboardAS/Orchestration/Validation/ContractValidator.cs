using System.Composition.Hosting;
using System.Reflection;
using Core.Attributes;

namespace Orchestration.Validation;

public static class ContractValidator
{
    public static void Validate(CompositionHost container)
    {
        // Scan all loaded types for [Requires] attributes and verify [Provides] exist
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } });

        var provided = allTypes
            .SelectMany(t => t.GetCustomAttributes<ProvidesAttribute>())
            .Select(p => p.Contract)
            .ToHashSet();

        foreach (var type in allTypes)
        {
            foreach (var req in type.GetCustomAttributes<RequiresAttribute>())
            {
                if (!provided.Contains(req.Contract))
                    throw new InvalidOperationException(
                        $"Contract validation failed: {type.Name} requires {req.Contract.Name} but no component provides it.");
            }
        }
    }
}
