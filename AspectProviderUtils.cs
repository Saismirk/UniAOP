using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public static class AspectProviderUtils {
    public static IAspectProvider GetAspectProvider(AttributeData attribute) =>
        attribute?.AttributeClass?.BaseType?.ToDisplayString() switch {
            "UniAOP.Runtime.MethodEnterAspectAttribute" => new MethodEnterAspectProvider(),
            "UniAOP.Runtime.MethodExitAspectAttribute" => new MethodExitAspectProvider(),
            "UniAOP.Runtime.MethodBoundaryAspectAttribute" => new MethodBoundaryAspectProvider(),
            // "UniAOP.Runtime.ExceptionAspectAttribute" => new ExceptionAspectProvider(),
            "UniAOP.Runtime.MethodValidationAspectAttribute" => new MethodValidationAspectProvider(),
            "UniAOP.Runtime.MethodValidationAsyncAspectAttribute" => new MethodValidationAsyncAspectProvider(),
            _ => null
        };

    public static void RecursiveGenerate(ref StringBuilder sourceBuilder,
        string methodName,
        string methodArgs,
        string methodReturnType,
        string methodModifiers,
        IEnumerator<AttributeData> attributeDataIter,
        bool isAsync) {
        if (attributeDataIter.MoveNext()) {
            GetAspectProvider(attributeDataIter.Current)?.Generate(ref sourceBuilder, 
                                                                   methodName, 
                                                                   methodArgs, 
                                                                   methodReturnType,
                                                                   methodModifiers, 
                                                                   attributeDataIter, 
                                                                   isAsync);
        } else {
            sourceBuilder.AppendLine($@"            {(isAsync ? "await " : "")}{methodName}({methodArgs});");
        }
    }
}