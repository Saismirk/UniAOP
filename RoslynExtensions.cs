using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public static class RoslynExtensions {
    public static ImmutableArray<ISymbol> ExplicitOrImplicitInterfaceImplementations(this ISymbol symbol)
    {
        if (symbol.Kind != SymbolKind.Method && symbol.Kind != SymbolKind.Property && symbol.Kind != SymbolKind.Event)
            return ImmutableArray<ISymbol>.Empty;

        var containingType = symbol.ContainingType;
        var query = from iface in containingType.AllInterfaces
            from interfaceMember in iface.GetMembers()
            let impl = containingType.FindImplementationForInterfaceMember(interfaceMember)
            where SymbolEqualityComparer.Default.Equals(symbol, impl)
            select interfaceMember;
        return query.ToImmutableArray();
    }
}