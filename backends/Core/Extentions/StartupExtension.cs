using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProtekTivCore.Extentions;

public static class StartupExtension
{
    public static IServiceCollection RegisterHttpClientsForUriResources(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var uriResources = configuration.GetSection("UriResources");

        foreach (var configSection in uriResources.GetChildren())
        {
            var resourceSection = configuration.GetSection(configSection.Path);
            var useHttpClient = resourceSection.GetValue<bool>("UseHttpClient");

            if (useHttpClient)
            {
                var childSection = configuration.GetSection(configSection.Path);
                var baseUrl = new Uri(configSection.Value ?? childSection.GetValue<string>("BaseUrl") ?? string.Empty);

                services.AddHttpClient(configSection.Key, _ =>
                {
                    _.BaseAddress = baseUrl;
                    var apiKeyHeader = childSection.GetValue<string>("ApiKeyRequestHeader");
                    if (!string.IsNullOrWhiteSpace(apiKeyHeader))
                    {
                        var apiKey = childSection.GetValue<string>("ApiKey");
                        _.DefaultRequestHeaders.Add(apiKeyHeader, apiKey);
                    }
                });
            }
        }

        return services;
    }
}
