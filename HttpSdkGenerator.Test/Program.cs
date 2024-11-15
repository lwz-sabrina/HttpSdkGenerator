﻿using HttpSdkGenerator.Abstract;
using Microsoft.Extensions.DependencyInjection;

// 设置依赖注入
var serviceProvider = new ServiceCollection()
    .AddLogging() // 添加控制台日志
    .AddHttpClient() // 注册 IHttpClientFactory
    .AddTransient<HttpSdkManager>()
    .BuildServiceProvider();
var manager = serviceProvider.GetRequiredService<HttpSdkManager>();
var users = await manager.CreateOrUpdateProcess(
    new Guid("99d07f0c-edd4-44f1-ad4b-b7aa6de577d8"),
    "chrome",
    "谷歌浏览器",
    true
);
var content = await users.Content.ReadAsStringAsync();
Console.WriteLine(content);
