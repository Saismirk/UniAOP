using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UniAOP;

public class SyntaxContextReceiver : ISyntaxContextReceiver {
    internal static ISyntaxContextReceiver Create() => new SyntaxContextReceiver();
    public static readonly HashSet<string> ValidAspects = new() { 
        "UniAOP.Runtime.MethodEnterAspectAttribute", 
        "UniAOP.Runtime.MethodExitAspectAttribute", 
        "UniAOP.Runtime.MethodBoundaryAspectAttribute" 
    };
    public List<(ISymbol methodSymbol, MethodDeclarationSyntax classSyntax)> ValidMethods { get; } = new();
    public StringBuilder Logs { get; } = new();
    public int MethodsVisited { get; set; }

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        try {
            var node = context.Node;
            if (node is not MethodDeclarationSyntax methodDeclaration 
                || methodDeclaration.AttributeLists.Count is 0
                || methodDeclaration.Modifiers.Any(m => m.ToString() is "public")) {
                return;
            }
            
            MethodsVisited++;
            var methodSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclaration);
            if (methodSymbol is null) {
                return;
            }
            
            Logs.AppendLine($"Visiting Method: {methodSymbol.ToDisplayString()}");
            foreach (var attribute in methodSymbol.GetAttributes()) {
                Logs.AppendLine(attribute.AttributeClass?.BaseType?.ToDisplayString());
            }
            if (methodSymbol.GetAttributes()
                            .Any(IsValidAttribute)) {
                ValidMethods.Add((methodSymbol, methodDeclaration));
            }
        }
        catch (Exception e) {
            Logs.AppendLine(e.ToString());
            Logs.AppendLine(e.StackTrace);
        }
    }

    private static bool IsValidAttribute(AttributeData attributeData) => ValidAspects.Contains(attributeData.AttributeClass?.BaseType?.ToDisplayString());
}