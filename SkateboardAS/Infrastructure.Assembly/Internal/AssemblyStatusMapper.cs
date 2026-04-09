using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Models;

namespace Infrastructure.Assembly.Internal;

internal record AssemblyStatusPayload(
    [property: JsonPropertyName("operation")] string Operation,
    [property: JsonPropertyName("progress")] double Progress,
    [property: JsonPropertyName("healthy")] bool Healthy,
    [property: JsonPropertyName("state")] int State
);

internal class AssemblyStatusMapper
{
    public AssemblyStatus Map(string mqttPayload)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<AssemblyStatusPayload>(mqttPayload);
            if (payload is null)
                return new AssemblyStatus();

            return new AssemblyStatus
            {
                CurrentOperation = payload.Operation,
                Progress = payload.Progress,
                IsHealthy = payload.Healthy,
                State = payload.State
            };
        }
        catch (JsonException)
        {
            return new AssemblyStatus();
        }
    }
}
