using ProtekTiv.Core.Interfaces.Services;

namespace ProtekTiv.Core.Services.ApiService;

public class Service<TResponse>(HttpClient httpClient) : IApiService<TResponse>
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<TResponse> GetAsync(string url)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<TResponse>();
    }

    public async Task<TResponse> GetAllAsync<TRequest>(IEnumerable<TRequest> request)
    {
        var tasks = request.Select(_ => _httpClient.GetAsync(_?.ToString()));
        IEnumerable<HttpResponseMessage> response = await Task.WhenAll(tasks);
        return (TResponse)response;
    }

    public async Task<TResponse> PostAsync(string url, TResponse item)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, item);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<TResponse>();
    }

    public async Task<TResponse> PutAsync(string url, TResponse item)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(url, item);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<TResponse>();
    }

    public async Task DeleteAsync(string url)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}