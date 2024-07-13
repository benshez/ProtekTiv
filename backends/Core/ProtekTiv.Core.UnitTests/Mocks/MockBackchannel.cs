using System.Reflection;

namespace ProtekTiv.Core.Interfaces.Mocks;

public class MockBackchannel : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri.AbsoluteUri.Equals("https://inmemory.microsoft.com/common/.well-known/openid-configuration"))
        {
            return await EmbeddedResourceReader.GetOpenIdConfigurationAsResponseMessage(GetType(), "microsoft-openid-config.json");
        }
        if (request.RequestUri.AbsoluteUri.Equals("https://inmemory.microsoft.com/common/discovery/keys"))
        {
            return await EmbeddedResourceReader.GetOpenIdConfigurationAsResponseMessage(GetType(),"microsoft-wellknown-keys.json");
        }

        throw new NotImplementedException();
    }

    public static class EmbeddedResourceReader
    {
        /// <summary>
        /// Reads the specified embedded resource from a given assembly.
        /// </summary>
        /// <param name="assemblyContainingType">Any <see cref="Type"/> in the assembly whose resource is to be read.</param>
        /// <param name="path">The path of the resource to be read.</param>
        /// <returns>The contents of the resource.</returns>
        public static Task<string> Read(Type assemblyContainingType, string path)
        {
            var asm = assemblyContainingType.GetTypeInfo().Assembly;
            var embeddedResourceName = asm.GetName().Name + path.Replace("/", ".");
            using var stream = asm.GetManifestResourceStream(embeddedResourceName);

            if (stream != null)
            {
                using var sr = new StreamReader(stream);
                stream.Dispose();
                var result = Task.FromResult(sr.ReadToEnd());
                sr.Dispose();
                return result;
            }

            return Task.FromResult(string.Empty);
        }

        public static async Task<HttpResponseMessage> GetOpenIdConfigurationAsResponseMessage(Type assemblyContainingType, string path)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(await Read(assemblyContainingType, path))
            };
        }
    }
}
