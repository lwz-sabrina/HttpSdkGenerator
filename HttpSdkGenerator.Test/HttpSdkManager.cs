using HttpSdkGenerator.Core;

namespace HttpSdkGenerator.Test
{
    public partial class HttpSdkManager : AbstractHttpSdkManager
    {
        //protected override Uri? BaseUrl => new Uri("http://localhost:12580");

        [HttpMapping(Method.Get, "http://localhost:12580/api/Cockpit/Result")]
        public partial Task<HttpResponseMessage> GetUsers();

        [HttpMapping(Method.Get, "http://localhost:12580/api/ProcessFilter/GetById?Id={Id}")]
        public partial Task<HttpResponseMessage> GetProcessById(
            Guid Id,
            CancellationToken cancellationToken
        );

        /// <summary>
        /// 转换后参数 new StringContent(new { id, name, replaceName, isWatch }, Encoding.UTF8, "application/json");
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="replaceName"></param>
        /// <param name="isWatch"></param>
        /// <returns></returns>
        [HttpMapping(
            Method.Post,
            "http://localhost:12580/api/ProcessFilter/CreateOrUpdate",
            ContentType.Json
        )]
        public partial Task<HttpResponseMessage> CreateOrUpdateProcess(
            Guid id,
            string name,
            string replaceName,
            bool isWatch
        );

        [HttpMapping(
            Method.Post,
            "https://open.ys7.com/api/lapp/token/get",
            ContentType.FormUrlEncoded
        )]
        public partial Task<HttpResponseMessage> GetToken(string appKey, string appSecret);
    }
}
