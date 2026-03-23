using System.Composition;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Agv;

[Export(typeof(IComponentUIDescriptor))]
public class AgvUIDescriptor : IComponentUIDescriptor
{
    public string ComponentType => "AGV";

    public ComponentCardModel GetCardModel() => new()
    {
        Name = "AGV Service",
        Icon = "truck",
        MachineType = "AGV",
        Description = "Automated Guided Vehicle for part transport",
        StatusFields = new List<string> { "Position", "BatteryLevel", "CurrentProgram" },
        Actions = new List<string> { "Start", "Stop", "Reset" }
    };

    public IEnumerable<string> GetAvailableActions() => new[] { "Start", "Stop", "Reset" };
    public IEnumerable<string> GetDisplayedStatusFields() => new[] { "Position", "BatteryLevel", "CurrentProgram" };
}
