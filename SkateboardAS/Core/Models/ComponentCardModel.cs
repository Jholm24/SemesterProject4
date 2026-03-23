namespace Core.Models;

public class ComponentCardModel
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IList<string> StatusFields { get; set; } = new List<string>();
    public IList<string> Actions { get; set; } = new List<string>();
}
