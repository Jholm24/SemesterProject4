namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ProvidesAttribute : Attribute
{
    public Type Contract { get; }
    public ProvidesAttribute(Type contract) { Contract = contract; }
}
