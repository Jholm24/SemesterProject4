using System.Net.Http.Json;
using Core.Models;

namespace Infrastructure.Agv.Internal;

internal class AgvHttpClient
{
    private const string BaseUrl = "http://localhost:8082/v1";
    private readonly HttpClient _http = new() { BaseAddress = new Uri(BaseUrl) };
    private readonly AgvStatusMapper _mapper = new();

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/v1/status", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<CommandResult> LoadProgramAsync(string programName, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync("/v1/load", new { name = programName }, ct);
        return response.IsSuccessStatusCode
            ? CommandResult.Ok()
            : CommandResult.Fail($"LoadProgram failed: {response.StatusCode}");
    }

    public async Task<CommandResult> ExecuteProgramAsync(CancellationToken ct = default)
    {
        var response = await _http.PutAsync("/v1/execute", null, ct);
        return response.IsSuccessStatusCode
            ? CommandResult.Ok()
            : CommandResult.Fail($"ExecuteProgram failed: {response.StatusCode}");
    }

    public async Task<AgvStatus> GetStatusAsync(CancellationToken ct = default)
    {
        var raw = await _http.GetFromJsonAsync<AgvStatusResponse>("/v1/status", ct);
        return raw is null ? new AgvStatus() : _mapper.Map(raw);
    }
}
