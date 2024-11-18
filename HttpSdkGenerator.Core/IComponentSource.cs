using System.Linq;
using Microsoft.CodeAnalysis;

namespace HttpSdkGenerator.Core
{
    public record GroupClass
    {
        public string NamespaceName { get; set; } = "";
        public string ClassName { get; set; } = "";
    }

    public interface IComponentSource
    {
        string GetSource(
            Compilation compilation,
            IGrouping<GroupClass, IMethodSymbol> methodSymbol
        );
    }
}
