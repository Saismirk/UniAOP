using System;
using System.Collections.Generic;
using System.Linq;
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
        ProcessAttributes(context, receiver);

#if DEBUG
        Utils.SaveSourceToPath("E:/Logs.txt", Logs.ToString());
#endif
    }

    private static void ProcessAttributes(GeneratorExecutionContext context, SyntaxContextReceiver receiver) {
        _context = context;
        try {
            if (receiver.ValidMethods.Count is 0) {
                return;
            }

            foreach (var methodData in receiver.ValidMethods) {
                var attributeData = methodData.methodSymbol
                                              .GetAttributes()
                                              .Where(a => SyntaxContextReceiver.ValidAspects.Contains(a.AttributeClass?.BaseType?.ToDisplayString()));
                Logs.AppendLine($"Processing: {methodData.methodSymbol.ToDisplayString()}");
                var classSource = ProcessClasses(methodData.methodSymbol, attributeData, methodData.classSyntax);
                var attributeName = methodData.methodSymbol.Name;
                var attributeDisplayName = attributeName.Replace("Attribute", "").Replace("UniAOP.", "");
                if (string.IsNullOrEmpty(classSource)) {
                    Logs.AppendLine($"{methodData.methodSymbol.ToDisplayString()} class generation failed for {attributeName}");
                    continue;
                }

                var namespaceName = methodData.methodSymbol.ContainingNamespace?.Name ?? "global";
                var className = methodData.methodSymbol.ContainingSymbol.Name;
                context.AddSource($"{namespaceName}_{className}_{attributeDisplayName}.g.cs", SourceText.From(classSource, Encoding.UTF8));
#if DEBUG
                Utils.SaveSourceToPath($"E:/{namespaceName}_{className}_{attributeDisplayName}.g.txt", classSource);
#endif
            }
        }
        catch (Exception e) {
            Logs.AppendLine($"{e.Message}\n{e.StackTrace}");
        }
    }

    private static string ProcessClasses(ISymbol methodSymbol, IEnumerable<AttributeData> attributeData, MethodDeclarationSyntax classSyntax) {
        var source = new StringBuilder();

        try {
            if (!GenerateClasses(methodSymbol, attributeData, classSyntax, source)) return string.Empty;
        }
        catch (Exception e) {
            source.AppendLine($@"/*Error: {e.Message}*/");
            source.AppendLine($@"/*{e.StackTrace}*/");
        }

        return source.ToString();
    }

    private static bool GenerateClasses(ISymbol methodSymbol, IEnumerable<AttributeData> attributeData, MethodDeclarationSyntax methodSyntax,
        StringBuilder sourceBuilder) {
        var attributes = methodSymbol.GetAttributes();
        if (attributes.Count() is 0) {
            Logs.AppendLine($"Skipping: {methodSymbol.ToDisplayString()}. No attributes found:");
            return false;
        }

        if (attributeData is null) {
            Logs.AppendLine($"Skipping: {methodSymbol.ToDisplayString()}. No valid attribute found.");
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
        var methodNameAop = methodName[0] is '_' ? methodName.Substring(1) : $"_{methodName}";
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

        sourceBuilder.AppendLine($@"    public partial class {className} {{
        {methodModifiers} {methodReturnType} {methodNameAop} ({methodArgs}) {{");
        if (!AspectProviderUtils.IsVoid(methodReturnType)) {
            var returnType = AspectProviderUtils.GetTaskReturnType(methodReturnType);
            sourceBuilder.AppendLine($@"            {returnType} result = default({returnType});");
        }
        using var attributeIterator = attributeData.GetEnumerator();
        AspectProviderUtils.RecursiveGenerate(ref sourceBuilder, methodName, methodArgs, methodReturnType, methodModifiers, attributeIterator, isAsync);
        if (!AspectProviderUtils.IsVoid(methodReturnType)) {
            sourceBuilder.AppendLine($@"            return result;");
        }
        sourceBuilder.AppendLine($@"        }}");
        sourceBuilder.AppendLine(namespaceSymbol.IsGlobalNamespace ? "}" : "\t}");
        if (!namespaceSymbol.IsGlobalNamespace) {
            sourceBuilder.AppendLine("}");
        }

        return true;
    }
}