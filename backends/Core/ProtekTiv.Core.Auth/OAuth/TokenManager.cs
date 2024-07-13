using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using ProtekTiv.Core.Interfaces.Caching.MemoryCache;
using ProtekTiv.Core.Interfaces.ApplicationSettings;

namespace ProtekTiv.Core.Auth.OAuth;

public class TokenManager
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private readonly IApplicationSettings _appSettings;
    private readonly ICacheProvider _cache;
    private readonly HttpClient _httpClient;

    public TokenManager(
        IApplicationSettings appSettings,
        HttpClient httpClient,
        ICacheProvider cache)
    {
        _appSettings = appSettings;
        _cache = cache;
        _httpClient = httpClient;
        _httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetAuthToken(string tokenCachekey, string audience)
    {
        await SemaphoreSlim.WaitAsync();

        try
        {
            return await _cache.GetOrCacheFromResult(tokenCachekey, async () =>
            {
                return await GenerateAuthAccessToken(audience);
            }, DateTimeOffset.Now.AddHours(12));
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private async Task<string> GenerateAuthAccessToken(string audience)
    {
        var responseMessage = await _httpClient.SendAsync(GenerateAuthTokenRequest(audience));
        var responseContentAsString = await responseMessage.Content.ReadAsStringAsync();

        responseMessage.EnsureSuccessStatusCode();

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContentAsString);

        if (tokenResponse != null) return string.IsNullOrEmpty(tokenResponse.AccessToken) ? string.Empty : tokenResponse.AccessToken;

        return string.Empty;

    }

    private HttpRequestMessage GenerateAuthTokenRequest(string audience)
    {
        var content = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                {"client_id", string.IsNullOrEmpty(_appSettings.Auth0ClientId) ? string.Empty : _appSettings.Auth0ClientId},
                {"client_secret", string.IsNullOrEmpty(_appSettings.Auth0ClientSecret) ? string.Empty : _appSettings.Auth0ClientSecret},
                {"audience", audience},
                {"grant_type", "client_credentials"}
            });

        var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.Auth0AuthorityHost)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        return request;
    }
}
