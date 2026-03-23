namespace Shared.Entities;

public class Formula
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public IList<string> RequiredComponentTypes { get; set; } = new List<string>();
    public string Description { get; set; } = string.Empty;
}
