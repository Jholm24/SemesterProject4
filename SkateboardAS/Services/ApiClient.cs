using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace SkateboardAS.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly NavigationManager _nav;

    public ApiClient(IHttpClientFactory factory, NavigationManager nav)
    {
        _factory = factory;
        _nav = nav;
    }

    private HttpClient Build()
    {
        var client = _factory.CreateClient();
        client.BaseAddress = new Uri(_nav.BaseUri);
        return client;
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        var response = await Build().GetAsync(path);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string path, T body)
        => await Build().PostAsJsonAsync(path, body);

    public async Task<HttpResponseMessage> PutAsync<T>(string path, T body)
        => await Build().PutAsJsonAsync(path, body);

    public async Task<HttpResponseMessage> DeleteAsync(string path)
        => await Build().DeleteAsync(path);
}
