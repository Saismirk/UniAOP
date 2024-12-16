using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public static class AspectProviderUtils {
    public static IAspectProvider GetAspectProvider(AttributeData attribute) =>
        attribute?.AttributeClass?.BaseType?.ToDisplayString() switch {
            "UniAOP.MethodEnterAspectAttribute" => new MethodEnterAspectProvider(),
            "UniAOP.MethodExitAspectAttribute" => new MethodExitAspectProvider(),
            "UniAOP.MethodBoundaryAspectAttribute" => new MethodBoundaryAspectProvider(),
            "UniAOP.ExceptionAspectAttribute" => new ExceptionAspectProvider(),
            "UniAOP.MethodValidationAspectAttribute" => new MethodValidationAspectProvider(),
            "UniAOP.MethodValidationAsyncAspectAttribute" => new MethodValidationAsyncAspectProvider(),
            _ => null
        };

    public static void RecursiveGenerate(ref StringBuilder sourceBuilder,
        string methodName,
        string methodArgs,
        string methodReturnType,
        string methodModifiers,
        IEnumerator<AttributeData> attributeDataIter,
        bool isAsync,
        int indent = 0) {
        if (attributeDataIter.MoveNext()) {
            GetAspectProvider(attributeDataIter.Current)?.Generate(ref sourceBuilder, 
                                                                   methodName, 
                                                                   methodArgs, 
                                                                   methodReturnType,
                                                                   methodModifiers, 
                                                                   attributeDataIter, 
                                                                   isAsync);
        } else {
            var indentStr = new string('\t', indent);
            sourceBuilder.AppendLine($@"            {indentStr}{(IsVoid(methodReturnType) ? "" : "result = ")}{(isAsync ? "await " : "")}{methodName}({methodArgs});");
        }
    }

    public static bool IsVoid(string methodReturnType) => methodReturnType == "void" 
                                                          || methodReturnType.EndsWith("UniTaskVoid")
                                                          || methodReturnType.EndsWith("UniTask")
                                                          || methodReturnType.EndsWith("Task");

    public static string GetTaskReturnType(string methodReturnType) {
        if (methodReturnType.StartsWith("System.Threading.Tasks.Task<")) return methodReturnType.Substring("System.Threading.Tasks.Task<".Length, methodReturnType.Length - "System.Threading.Tasks.Task<".Length - 1);
        if (methodReturnType.StartsWith("Cysharp.Threading.Tasks.UniTask<")) return methodReturnType.Substring("UniTask<".Length, methodReturnType.Length - "UniTask<".Length - 1);
        return methodReturnType;
    }
}