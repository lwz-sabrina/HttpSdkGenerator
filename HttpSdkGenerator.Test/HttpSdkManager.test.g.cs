//using Microsoft.Extensions.Logging;

//namespace HttpSdkGenerator.Abstract
//{
//    partial class HttpSdkManager
//    {
//#nullable enable
//        private readonly ILogger<HttpSdkManager> _logger;
//        private readonly IHttpClientFactory _clientFactory;

//        public HttpSdkManager(
//            ILogger<HttpSdkManager> logger,
//            IHttpClientFactory clientFactory
//        )
//        {
//            _logger = logger;
//            _clientFactory = clientFactory;
//        }

//        private async Task<HttpResponseMessage> SendAsync(
//            string uri,
//            HttpMethod method,
//            HttpContent? content = null,
//            CancellationToken cancellationToken = default
//        )
//        {
//            using var _client = _clientFactory.CreateClient();
//            ConfigRequestHeaders(_client.DefaultRequestHeaders);
//            var request = new HttpRequestMessage
//            ();
//            request.Method = method;
//            request.RequestUri = BaseUrl is null ? new Uri(uri) : new Uri(BaseUrl, uri);
//            request.Content = content;
//            var response = await _client.SendAsync(request);
//            response.EnsureSuccessStatusCode();
//            _logger.LogInformation($"Request: {request.Method} {request.RequestUri}");
//            return response;
//        }
//        public partial Task<HttpResponseMessage> GetToken(string appKey, string appSecret) =>
//        SendAsync($"https://open.ys7.com/api/lapp/token/get",
//            new HttpMethod("POST"),
//            new FormUrlEncodedContent([new KeyValuePair<string, string>("appKey", appKey), new KeyValuePair<string, string>("appSecret", appSecret)]),
//            default);
//    }
//}