using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpSdkGenerator.Core
{
    internal class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        public static SyntaxContextReceiver GetInstance() =>
            SyntaxContextReceiverConstructor.Instance;

        private class SyntaxContextReceiverConstructor
        {
            internal static readonly SyntaxContextReceiver Instance = new SyntaxContextReceiver();
        }

        public List<IMethodSymbol> MethodSymbols { get; set; } = new List<IMethodSymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var syntaxNode = context.Node;
            if (syntaxNode is not MethodDeclarationSyntax methodDeclarationSyntax)
            {
                return;
            }
            if (!methodDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }
            var semanticModel = context.SemanticModel;
            var compilation = semanticModel.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(
                typeof(HttpMappingAttribute).FullName
            );
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (attributeSymbol is null)
            {
                return;
            }
            if (methodSymbol is null)
            {
                return;
            }
            if (!methodSymbol.GetAttributes().Any(predicate))
            {
                return;
            }
            if (methodSymbol.ContainingType.TypeKind != TypeKind.Class)
            {
                return;
            }
            MethodSymbols.Add(methodSymbol);
            bool predicate(AttributeData x) =>
                SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol);
        }
    }
}
