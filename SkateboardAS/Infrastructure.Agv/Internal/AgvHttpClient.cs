using System.Net.Http.Json;
using Core.Models;

namespace Infrastructure.Agv.Internal;

internal class AgvHttpClient
{
    private const string BaseUrl = "http://localhost:8082/v1";
    private readonly HttpClient _http = new();
    public string Name  = "AGV";

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        
        _http.BaseAddress = new Uri(BaseUrl);
        var response = await _http.GetAsync($"{BaseUrl}/staus", ct);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
        var request  = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/status");
        var responseReturn = await _http.SendAsync(request, ct);
        responseReturn.EnsureSuccessStatusCode();
        //Implement "if" statement if needed
        
    }

    public async Task<CommandResult> LoadProgramAsync(string programName, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"{BaseUrl}/load/, new {Name = programName}", ct);
        response.EnsureSuccessStatusCode();
        return CommandResult.Ok();
    }

    public async Task<CommandResult> ExecuteProgramAsync(CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"{BaseUrl}/execute", ct);
        response.EnsureSuccessStatusCode();
        return CommandResult.Ok();
    }

    public async Task<AgvStatus> GetStatusAsync(CancellationToken ct = default)
    {
        var status = await _http.GetFromJsonAsync<AgvStatus>($"{BaseUrl}/status", ct);
        return status ?? throw new InvalidOperationException("Received empty staturs from AGV.");
    }
}
