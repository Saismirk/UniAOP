using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public class MethodValidationAspectProvider : IAspectProvider {
    public void Generate(ref StringBuilder sourceBuilder,
        string methodName,
        string methodArgs,
        string methodReturnType,
        string methodModifiers,
        IEnumerator<AttributeData> attributeDataIter,
        bool isAsync) {
        var attributeData = attributeDataIter.Current;
        if (attributeData?.AttributeClass is null) return;
        sourceBuilder.AppendLine($@"
            var  _{attributeData.AttributeClass.Name} = new {attributeData.AttributeClass.Name}();
            if (!_{attributeData.AttributeClass.Name}.ValidateMethod({methodArgs})) {{
                return {(AspectProviderUtils.IsVoid(methodReturnType) ? "" : "result")};
            }};
");
        AspectProviderUtils.RecursiveGenerate(ref sourceBuilder, methodName, methodArgs, methodReturnType, methodModifiers, attributeDataIter, isAsync);
    }
}