using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProtekTiv.Core.Extentions.HttpClient;

public static class HttpClientExtention
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services,
       IConfiguration configuration)
    {
        var resources = configuration.GetSection("UriResources");

        resources.GetChildren().ToList().ForEach(resource =>
        {
            var section = configuration.GetSection(resource.Path);
            var useResource = section.GetValue<bool>("UseHttpClient");
            var baseUrl = section.GetValue<string>("BaseUrl");
            var apiKeyHeader = section.GetValue<string>("ApiKeyRequestHeader");
            var apiKey = section.GetValue<string>("ApiKey");

            if (useResource)
            {
                services.AddHttpClient(resource.Key, _ =>
                {
                    if (!string.IsNullOrWhiteSpace(baseUrl))
                    {
                        _.BaseAddress = new Uri(baseUrl);
                    }

                    if (!string.IsNullOrWhiteSpace(apiKeyHeader))
                    {
                        _.DefaultRequestHeaders.Add(apiKeyHeader, apiKey);
                    }
                });
            }
        });

        return services;
    }
}