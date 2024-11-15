using System.Net.Http.Headers;

namespace HttpSdkGenerator.Abstract
{
    public abstract class AbstractHttpSdkManager
    {
        protected virtual Uri? BaseUrl { get; }

        public virtual void ConfigRequestHeaders(HttpRequestHeaders headers) { }
    }
}
