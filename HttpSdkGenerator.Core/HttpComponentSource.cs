using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpSdkGenerator.Core
{
    internal class HttpComponentSource : IComponentSource
    {
        private object GetAttributeArgument(
            IMethodSymbol methodSymbol,
            ISymbol attributeSymbol,
            int index
        )
        {
            var attributes = methodSymbol.GetAttributes();
            var attribute = attributes
                .Where(x =>
                    SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol)
                )
                .Last();
            var argument = attribute.ConstructorArguments.ElementAt(index);
            return argument.Value ?? throw new ArgumentException("Attribute argument is null");
        }

        public string GetSource(
            Compilation compilation,
            IGrouping<GroupClass, IMethodSymbol> methodSymbols
        )
        {
            var namespaceName = methodSymbols.Key.NamespaceName;
            var className = methodSymbols.Key.ClassName;
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
                var attributeSymbol =
                    compilation.GetTypeByMetadataName(typeof(HttpSdkMethodAttribute).FullName)
                    ?? throw new ArgumentException("Cannot find HttpSdkMethodAttribute");
                var method = GetAttributeArgument(methodSymbol, attributeSymbol, 0).ToString();
                var uri = GetAttributeArgument(methodSymbol, attributeSymbol, 1).ToString();

                switch (method.ToLower())
                {
                    case "get":
                        builder.Append(GetMethodSource(methodSymbol, uri, method));
                        break;
                    case "post":
                        var contentType = GetAttributeArgument(methodSymbol, attributeSymbol, 2)
                            .ToString();
                        builder.Append(PostMethodSource(methodSymbol, uri, method, contentType));
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

        private string GetMethodSource(IMethodSymbol methodSymbol, string uri, string method)
        {
            var methodSyntax = (MethodDeclarationSyntax)
                methodSymbol.DeclaringSyntaxReferences.First().GetSyntax();
            // 获取CancellationToken参数
            var cancellationTokenParameter =
                methodSymbol
                    .Parameters.FirstOrDefault(x => x.Type.Name == "CancellationToken")
                    ?.Name ?? "default";
            return $$"""
{{methodSyntax.Modifiers}} Task<HttpResponseMessage> {{methodSymbol.Name}}({{methodSyntax
                    .ParameterList
                    .Parameters}}) =>
        SendAsync($"{{uri}}", new HttpMethod("{{method}}"), null, {{cancellationTokenParameter}});

""";
        }

        private string PostMethodSource(
            IMethodSymbol methodSymbol,
            string uri,
            string method,
            string contentType
        )
        {
            var methodSyntax = (MethodDeclarationSyntax)
                methodSymbol.DeclaringSyntaxReferences.First().GetSyntax();
            var contentParameter = methodSymbol
                .Parameters.Where(x => x.Type.Name != typeof(CancellationToken).Name)
                .Select(x => x.Name);
            // 获取CancellationToken参数
            var cancellationTokenParameter =
                methodSymbol
                    .Parameters.FirstOrDefault(x => x.Type.Name == "CancellationToken")
                    ?.Name ?? "default";
            switch (contentType.ToLower())
            {
                case ContentType.Json:
                    return $$"""
{{methodSyntax.Modifiers}} Task<HttpResponseMessage> {{methodSymbol.Name}}({{methodSyntax
                    .ParameterList
                    .Parameters}}) =>
        SendAsync($"{{uri}}", 
            new HttpMethod("{{method}}"),
            new StringContent(
                JsonSerializer.Serialize(new { {{string.Join(", ", contentParameter)}} }),
                System.Text.Encoding.UTF8,
                "application/json"
            ) , 
            {{cancellationTokenParameter}});

""";
                case ContentType.FormUrlEncoded:
                    return $$"""
{{methodSyntax.Modifiers}} Task<HttpResponseMessage> {{methodSymbol.Name}}({{methodSyntax
                  .ParameterList
                  .Parameters}}) =>
        SendAsync($"{{uri}}", 
            new HttpMethod("{{method}}"),
            new FormUrlEncodedContent([{{string.Join(
                        ", ",
                        contentParameter.Select(x =>
                            $"new KeyValuePair<string, string>(\"{x}\", {x})"
                        )
                    )}}]) ,
            {{cancellationTokenParameter}});

""";
                default:
                    throw new ArgumentException($"Unsupported content type: {contentType}");
            }
        }
    }
}
