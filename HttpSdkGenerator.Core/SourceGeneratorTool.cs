using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpSdkGenerator.Core
{
    internal static class SourceGeneratorTool
    {
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
