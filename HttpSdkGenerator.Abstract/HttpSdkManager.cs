using HttpSdkGenerator.Core;

namespace HttpSdkGenerator.Abstract
{
    public partial class HttpSdkManager : AbstractHttpSdkManager
    {
        //protected override Uri? BaseUrl => new Uri("http://localhost:12580");

        //[HttpSdkMethod(Method.Get, "/api/Cockpit/Result")]
        //public partial Task<HttpResponseMessage> GetUsers();

        //[HttpSdkMethod(Method.Get, "/api/ProcessFilter/GetById?Id={Id}")]
        //public partial Task<HttpResponseMessage> GetProcessById(
        //    Guid Id,
        //    CancellationToken cancellationToken
        //);

        ///// <summary>
        ///// 转换后参数 new StringContent(new { id, name, replaceName, isWatch }, Encoding.UTF8, "application/json");
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="name"></param>
        ///// <param name="replaceName"></param>
        ///// <param name="isWatch"></param>
        ///// <returns></returns>
        //[HttpSdkMethod(Method.Post, "/api/ProcessFilter/CreateOrUpdate", ContentType.Json)]
        //public partial Task<HttpResponseMessage> CreateOrUpdateProcess(
        //    Guid id,
        //    string name,
        //    string replaceName,
        //    bool isWatch
        //);
        [HttpSdkMethod(
            Method.Post,
            "https://open.ys7.com/api/lapp/token/get",
            ContentType.FormUrlEncoded
        )]
        public partial Task<HttpResponseMessage> GetToken(string appKey, string appSecret);
    }
}
