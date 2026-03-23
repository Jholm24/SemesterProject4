namespace Shared.Entities;

public class ProductionLine
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public IList<string> AssignedComponentIds { get; set; } = new List<string>();
    public bool IsActive { get; set; }
}
