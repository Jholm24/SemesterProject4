using Core.Models;

namespace Infrastructure.Agv.Internal;

internal class AgvHttpClient
{
    private const string BaseUrl = "http://localhost:8082/v1";
    private readonly HttpClient _http = new();

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        // TODO: Implement - GET /status to verify AGV is reachable
        await Task.CompletedTask;
    }

    public async Task<CommandResult> LoadProgramAsync(string programName, CancellationToken ct = default)
    {
        // TODO: Implement - PUT /load with program name
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<CommandResult> ExecuteProgramAsync(CancellationToken ct = default)
    {
        // TODO: Implement - PUT /execute
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<AgvStatus> GetStatusAsync(CancellationToken ct = default)
    {
        // TODO: Implement - GET /status and map response
        await Task.CompletedTask;
        return new AgvStatus();
    }
}
