using System;
using System.Net.Http.Headers;

namespace HttpSdkGenerator.Core
{
    public abstract class AbstractHttpSdkManager
    {
        protected virtual Uri? BaseUrl { get; }

        public virtual void ConfigRequestHeaders(HttpRequestHeaders headers) { }
    }
}
