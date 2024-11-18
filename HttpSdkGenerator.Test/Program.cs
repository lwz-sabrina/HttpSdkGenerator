using HttpSdkGenerator.Abstract;
using Microsoft.Extensions.DependencyInjection;

// 设置依赖注入
var serviceProvider = new ServiceCollection()
    .AddLogging() // 添加控制台日志
    .AddHttpClient() // 注册 IHttpClientFactory
    .AddTransient<HttpSdkManager>()
    .BuildServiceProvider();
var manager = serviceProvider.GetRequiredService<HttpSdkManager>();
var users = await manager.GetToken(
    "06e21869c8774bf48837aaf14eee49d5",
    "8990d9f47e8cdb1b82ce19be23432af9"
);
var content = await users.Content.ReadAsStringAsync();
Console.WriteLine(content);
