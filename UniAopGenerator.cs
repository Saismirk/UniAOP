using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace UniAOP;

[Generator]
public class UniAopGenerator : ISourceGenerator {
    private static readonly StringBuilder Logs = new();
    private static GeneratorExecutionContext? _context;

    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }

    public void Execute(GeneratorExecutionContext context) {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver) return;
#if DEBUG
        Utils.SaveSourceToPath("E:/Error.txt",
                               $"Methods found: {receiver.ValidMethods.Count}."
                               + $"\nVisited: {receiver.MethodsVisited}"
                               + $"\n{receiver.Logs}");
#endif
        ProcessAttribute("MethodEnter", context, receiver);
        ProcessAttribute("MethodExit", context, receiver);
        ProcessAttribute("MethodBoundary", context, receiver);
#if DEBUG
        Utils.SaveSourceToPath("E:/Logs.txt", Logs.ToString());
#endif
    }

    private static void ProcessAttribute(string attribute, GeneratorExecutionContext context, SyntaxContextReceiver receiver) {
        _context = context;
        try {
            var attributeName = $"UniAOP.Runtime.{attribute}AspectAttribute";
            if (receiver.ValidMethods.Count is 0) {
                return;
            }

            foreach (var methodData in receiver.ValidMethods) {
                Logs.AppendLine($"Processing: {methodData.methodSymbol.ToDisplayString()}");
                var classSource = ProcessClass(methodData.methodSymbol, attributeName, methodData.classSyntax);
                if (string.IsNullOrEmpty(classSource)) {
                    Logs.AppendLine($"{methodData.methodSymbol.ToDisplayString()} class generation failed for {attributeName}");
                    continue;
                }

                var namespaceName = methodData.methodSymbol.ContainingNamespace?.Name ?? "global";
                var className = methodData.methodSymbol.Name;
                context.AddSource($"{namespaceName}_{className}_{attribute}.g.cs", SourceText.From(classSource, Encoding.UTF8));
#if DEBUG
                Utils.SaveSourceToPath($"E:/{namespaceName}_{className}_{attribute}.g.txt", classSource);
#endif
            }
        }
        catch (Exception e) {
            Logs.AppendLine($"{e.Message}\n{e.StackTrace}");
        }
    }

    private static string ProcessClass(ISymbol methodSymbol, string attributeName, MethodDeclarationSyntax classSyntax) {
        var source = new StringBuilder();

        try {
            if (!GenerateClass(methodSymbol, attributeName, classSyntax, source)) return string.Empty;
        }
        catch (Exception e) {
            source.AppendLine($@"/*Error: {e.Message}*/");
            source.AppendLine($@"/*{e.StackTrace}*/");
        }

        return source.ToString();
    }

    private static bool GenerateClass(ISymbol methodSymbol, string attributeName, MethodDeclarationSyntax methodSyntax, StringBuilder sourceBuilder) {
        var attributes = methodSymbol.GetAttributes();
        if (attributes.Count() is 0) {
            Logs.AppendLine($"Skipping: {methodSymbol.ToDisplayString()}. No attributes found:");
            return false;
        }

        var attributeData = attributes.FirstOrDefault(a => a.AttributeClass?.BaseType?.ToDisplayString() == attributeName);
        if (attributeData is null) {
            Logs.AppendLine($"Skipping: {methodSymbol.ToDisplayString()}. No {attributeName} attribute found.");
            foreach (var attribute in attributes) {
                Logs.AppendLine("\t- " + attribute.AttributeClass?.ToDisplayString());
            }

            return false;
        }

        var classDeclaration = (ClassDeclarationSyntax)methodSyntax.Parent!;
        if (classDeclaration == null) {
            Logs.AppendLine($"Skipping: {methodSymbol.ToDisplayString()}. No class found.");
            return false;
        }

        var namespaceName = ((NamespaceDeclarationSyntax)classDeclaration.Parent)?.Name.ToString() ?? "global";
        var className = classDeclaration.Identifier.Text;
        var isAsync = methodSyntax.Modifiers.Any(m => m.ToString() == "async");
        var methodName = methodSyntax.Identifier.Text;
        var methodArgs = string.Join(", ", methodSyntax.ParameterList.Parameters.Select(p => p.Identifier.Text));
        var methodReturnType = _context?.Compilation
                                       .GetSemanticModel(methodSyntax.ReturnType.SyntaxTree)
                                       .GetSymbolInfo(methodSyntax.ReturnType)
                                       .Symbol?
                                       .ToDisplayString()
                               ?? methodSyntax.ReturnType.ToString();
        var methodModifiers = methodSyntax.Modifiers.ToString().Replace("private", "public");
        var namespaceSymbol = methodSymbol.ContainingNamespace;
        if (!namespaceSymbol.IsGlobalNamespace) {
            sourceBuilder.AppendLine($@"// UniAOP Generated Class
using System.Collections.Generic;

namespace {namespaceName} {{");
        }

        sourceBuilder.AppendLine($@"    public partial class {className} {{");
        IAspectProvider aspectProvider = attributeName switch {
            "UniAOP.Runtime.MethodEnterAspectAttribute" => new MethodEnterAspectProvider(),
            "UniAOP.Runtime.MethodExitAspectAttribute" => new MethodExitAspectProvider(),
            "UniAOP.Runtime.MethodBoundaryAspectAttribute" => new MethodBoundaryAspectProvider(),
            _ => null
        };
        
        aspectProvider?.Generate(ref sourceBuilder, methodName, methodArgs, methodReturnType, methodModifiers, attributeData, isAsync);
        sourceBuilder.AppendLine(namespaceSymbol.IsGlobalNamespace ? "}" : "\t}");
        if (!namespaceSymbol.IsGlobalNamespace) {
            sourceBuilder.AppendLine("}");
        }

        return true;
    }
}