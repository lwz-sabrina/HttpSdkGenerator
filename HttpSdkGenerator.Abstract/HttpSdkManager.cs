using HttpSdkGenerator.Core;

namespace HttpSdkGenerator.Abstract
{
    public partial class HttpSdkManager : AbstractHttpSdkManager
    {
        protected override Uri? BaseUrl => new Uri("http://localhost:12580");

        [HttpSdkMethod("get", "/api/Cockpit/Result")]
        public partial Task<HttpResponseMessage> GetUsers();

        [HttpSdkMethod("get", "/api/ProcessFilter/GetById?Id={Id}")]
        public partial Task<HttpResponseMessage> GetProcessById(
            Guid Id,
            CancellationToken cancellationToken
        );

        [HttpSdkMethod("post", "/api/ProcessFilter/CreateOrUpdate", "application/json")]
        public partial Task<HttpResponseMessage> CreateOrUpdateProcess(
            Guid id,
            string name,
            string replaceName,
            bool isWatch
        );
    }
}
