﻿using HttpSdkGenerator.Test;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services
            .AddLogging() // 添加控制台日志
            .AddHttpClient() // 注册 IHttpClientFactory
            .AddTransient<HttpSdkManager>();
    });

// 设置依赖注入
var host = builder.Build();

var manager = host.Services.GetRequiredService<HttpSdkManager>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var users = await manager.GetToken(
    "06e21869c8774bf48837aaf14eee49d5",
    "8990d9f47e8cdb1b82ce19be23432af9"
);
var content = await users.Content.ReadAsStringAsync();
logger.LogInformation(content);
