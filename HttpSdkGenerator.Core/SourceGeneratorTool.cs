using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpSdkGenerator.Core
{
    internal static class SourceGeneratorTool
    {
        public static string GetRootSource(string namespaceName, string className)
        {
            return $$"""
using Microsoft.Extensions.Logging;

namespace {{namespaceName}}
{
    partial class {{className}}
    {
#nullable enable
        private readonly ILogger<{{className}}> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public {{className}}(
            ILogger<{{className}}> logger,
            IHttpClientFactory clientFactory
        )
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        private async Task<HttpResponseMessage> SendAsync(
            string uri,
            HttpMethod method,
            HttpContent? content,
            CancellationToken cancellationToken
        )
        {
            using var _client = _clientFactory.CreateClient();
            ConfigRequestHeaders(_client.DefaultRequestHeaders);
            var request = new HttpRequestMessage
            ();
            request.Method = method;
            request.RequestUri = BaseUrl is null? new Uri(uri) : new Uri(BaseUrl, uri);
            request.Content = content;
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation($"Request: {request.Method} {request.RequestUri}");
            return response;
        }
    }
}
""";
        }

        public static string GetHttpClientSource(
            string namespaceName,
            string className,
            Compilation compilation,
            IEnumerable<IMethodSymbol> methodSymbols
        )
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(
                $$"""
using System.Text.Json;

namespace {{namespaceName}}
{
    partial class {{className}}
    {
#nullable enable

"""
            );
            foreach (var methodSymbol in methodSymbols)
            {
                // 获取HttpSdkMethod特性信息
                var attribute = methodSymbol
                    .GetAttributes()
                    .LastOrDefault(x =>
                        SymbolEqualityComparer.Default.Equals(
                            x.AttributeClass,
                            compilation.GetTypeByMetadataName(
                                typeof(HttpSdkMethodAttribute).FullName
                            )
                        )
                    );
                if (attribute == null)
                {
                    continue;
                }
                var method = attribute.ConstructorArguments[0].Value!.ToString();
                var uri = attribute.ConstructorArguments[1].Value!.ToString();
                var contentType = attribute.ConstructorArguments[2].Value!.ToString();
                var methodSyntax = (MethodDeclarationSyntax)
                    methodSymbol.DeclaringSyntaxReferences.First().GetSyntax();
                // 获取方法参数列表
                var parameters = methodSyntax.ParameterList.Parameters;
                // 获取CancellationToken参数
                var cancellationTokenParameter =
                    parameters
                        .LastOrDefault(x =>
                            x.Type!.EqualsInheritsFrom<CancellationToken>(compilation)
                        )
                        ?.Identifier.Text ?? "default";
                switch (method.ToLower())
                {
                    case "get":
                        builder.Append(
                            $$"""
{{methodSyntax.Modifiers}} Task<HttpResponseMessage> {{methodSymbol.Name}}({{parameters}}) =>
        SendAsync($"{{uri}}", new HttpMethod("{{method}}"), null, {{cancellationTokenParameter}});

"""
                        );
                        break;
                    case "post":
                        var contentParameter = parameters
                            .Where(x => !x.Type!.EqualsInheritsFrom<CancellationToken>(compilation))
                            .Select(x => x.Identifier.Text);
                        string contentParameterString = string.Join(", ", contentParameter);
                        switch (contentType.ToLower())
                        {
                            case "application/json":
                                builder.Append(
                                    GetPostApplicationJson(
                                        methodSyntax,
                                        methodSymbol,
                                        uri,
                                        contentParameterString,
                                        method,
                                        parameters,
                                        cancellationTokenParameter
                                    )
                                );
                                break;
                            default:
                                throw new ArgumentException("Invalid content type");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unsupported HTTP method: {method}");
                }
            }
            builder.Append(
                $$"""
    }
}
"""
            );
            return builder.ToString();
        }

        public static string GetPostApplicationJson(
            MethodDeclarationSyntax methodSyntax,
            IMethodSymbol methodSymbol,
            string uri,
            string contentParameterString,
            string method,
            SeparatedSyntaxList<ParameterSyntax> parameters,
            string cancellationTokenParameter
        )
        {
            return $$"""
{{methodSyntax.Modifiers}} Task<HttpResponseMessage> {{methodSymbol.Name}}({{parameters}}) =>
        SendAsync($"{{uri}}", 
            new HttpMethod("{{method}}"),
            new StringContent(
                JsonSerializer.Serialize(new { {{contentParameterString}} }),
                System.Text.Encoding.UTF8,
                "application/json"
            ) , 
            {{cancellationTokenParameter}});

""";
        }

        public static bool EqualsInheritsFrom<T>(
            this TypeSyntax typeSyntax,
            Compilation compilation
        )
        {
            var httpContentSymbol = compilation.GetTypeByMetadataName(typeof(T).FullName);
            var typeInfo = compilation
                .GetSemanticModel(typeSyntax.SyntaxTree)
                .GetTypeInfo(typeSyntax);

            return typeInfo.Type != null
                && (
                    SymbolEqualityComparer.Default.Equals(typeInfo.Type, httpContentSymbol)
                    || typeInfo.Type.InheritsFrom(httpContentSymbol, compilation)
                );
        }

        // 扩展方法检查是否为子类
        private static bool InheritsFrom(
            this ITypeSymbol? type,
            ITypeSymbol? baseType,
            Compilation compilation
        )
        {
            while (type != null)
            {
                if (SymbolEqualityComparer.Default.Equals(type, baseType))
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}
