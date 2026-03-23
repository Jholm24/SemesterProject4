namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute
{
    public string Name { get; }
    public string Version { get; }
    public ComponentAttribute(string name, string version)
    {
        Name = name;
        Version = version;
    }
}
