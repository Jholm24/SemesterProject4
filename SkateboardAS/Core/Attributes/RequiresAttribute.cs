namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresAttribute : Attribute
{
    public Type Contract { get; }
    public RequiresAttribute(Type contract) { Contract = contract; }
}
