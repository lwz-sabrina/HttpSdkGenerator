using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace HttpSdkGenerator.Core
{
    [Generator(LanguageNames.CSharp)]
    public class SourceGenerator : ISourceGenerator
    {
        private Dictionary<string, IComponentSource> sources =
            new()
            {
                { "root", new RootComponentSource() },
                { "httpapi", new HttpComponentSource() }
            };

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
            var groupMethodSymbols = receiver
                .MethodSymbols.GroupBy(x => new GroupClass
                {
                    NamespaceName = x.ContainingNamespace.ToString(),
                    ClassName = x.ContainingType.Name
                })
                .ToList();
            foreach (var group in groupMethodSymbols)
            {
                // 获取方法的类名称
                var className = group.Key.ClassName;
                foreach (var source in sources)
                {
                    string clientApi = source.Value.GetSource(compilation, group);
                    context.AddSource($"{className}.{source.Key}.g.cs", clientApi);
                }
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
