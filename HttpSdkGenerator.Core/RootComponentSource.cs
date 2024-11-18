using System.Linq;
using Microsoft.CodeAnalysis;

namespace HttpSdkGenerator.Core
{
    internal class RootComponentSource : IComponentSource
    {
        public string GetSource(
            Compilation compilation,
            IGrouping<GroupClass, IMethodSymbol> methodSymbol
        )
        {
            var namespaceName = methodSymbol.Key.NamespaceName;
            var className = methodSymbol.Key.ClassName;
            return $$"""
using Microsoft.Extensions.Logging;

namespace {{namespaceName}}
{
    partial class {{className}}
    {
#nullable enable
        private readonly ILogger<{{className}}> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public {{className}}(
            ILogger<{{className}}> logger,
            IHttpClientFactory clientFactory
        )
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        private async Task<HttpResponseMessage> SendAsync(
            string uri,
            HttpMethod method,
            HttpContent? content,
            CancellationToken cancellationToken
        )
        {
            using var _client = _clientFactory.CreateClient();
            ConfigRequestHeaders(_client.DefaultRequestHeaders);
            var request = new HttpRequestMessage
            ();
            request.Method = method;
            request.RequestUri = BaseUrl is null? new Uri(uri) : new Uri(BaseUrl, uri);
            request.Content = content;
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation($"Request: {request.Method} {request.RequestUri}");
            return response;
        }
    }
}
""";
        }
    }
}
