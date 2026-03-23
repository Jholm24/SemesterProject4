using System.Composition;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Assembly;

[Export(typeof(IComponentUIDescriptor))]
public class AssemblyUIDescriptor : IComponentUIDescriptor
{
    public string ComponentType => "AssemblyStation";

    public ComponentCardModel GetCardModel() => new()
    {
        Name = "Assembly Service",
        Icon = "cog",
        MachineType = "AssemblyStation",
        Description = "UR assembly station for product assembly",
        StatusFields = new List<string> { "CurrentOperation", "Progress", "IsHealthy" },
        Actions = new List<string> { "StartOperation", "CheckHealth" }
    };

    public IEnumerable<string> GetAvailableActions() => new[] { "StartOperation", "CheckHealth" };
    public IEnumerable<string> GetDisplayedStatusFields() => new[] { "CurrentOperation", "Progress", "IsHealthy" };
}
