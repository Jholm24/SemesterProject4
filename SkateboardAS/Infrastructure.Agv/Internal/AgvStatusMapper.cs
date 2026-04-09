using System.Text.Json.Serialization;
using Core.Models;

namespace Infrastructure.Agv.Internal;

internal record AgvStatusResponse(
    [property: JsonPropertyName("position")] string Position,
    [property: JsonPropertyName("battery_percentage")] double BatteryPercentage,
    [property: JsonPropertyName("state_id")] int StateId,
    [property: JsonPropertyName("mission_text")] string MissionText
);

internal class AgvStatusMapper
{
    public AgvStatus Map(AgvStatusResponse response) => new()
    {
        Position = response.Position,
        BatteryLevel = response.BatteryPercentage,
        CurrentProgram = response.MissionText,
        State = response.StateId
    };
}
