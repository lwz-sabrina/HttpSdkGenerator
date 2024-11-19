using System;

namespace HttpSdkGenerator.Core
{
    public class HttpMappingAttribute : Attribute
    {
        public HttpMappingAttribute(
            string method,
            string httpUrl,
            string contentType = ContentType.None
        )
        {
            Method = method;
            HttpUrl = httpUrl;
            ContentTYpe = contentType;
        }

        public string Method { get; private set; }
        public string HttpUrl { get; private set; }

        public string ContentTYpe { get; private set; }
    }
}
