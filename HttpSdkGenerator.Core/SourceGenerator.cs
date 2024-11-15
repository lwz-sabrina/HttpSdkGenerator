using System.Linq;
using Microsoft.CodeAnalysis;

namespace HttpSdkGenerator.Core
{
    [Generator(LanguageNames.CSharp)]
    public class SourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
            {
                return;
            }
            if (receiver.MethodSymbols.Count == 0)
            {
                return;
            }
            var groupMethodSymbols = receiver.MethodSymbols.GroupBy(x => new
            {
                Namespace = x.ContainingNamespace.ToString(),
                ClassName = x.ContainingType.Name
            });
            foreach (var group in groupMethodSymbols)
            {
                // 获取方法的类名称
                var className = group.Key.ClassName;
                // 获取命名空间名称
                var namespaceName = group.Key.Namespace;
                string main = SourceGeneratorTool.GetRootSource(namespaceName, className);
                context.AddSource($"{className}.main.g.cs", main);
                string clientApi = SourceGeneratorTool.GetHttpClientSource(
                    namespaceName,
                    className,
                    compilation,
                    group
                );
                context.AddSource($"{className}.clientapi.cs", clientApi);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(SyntaxContextReceiver.GetInstance);

            // 开启调试器
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
        }
    }
}
