using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public class MethodExitAspectProvider : IAspectProvider {
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
            var _{attributeData.AttributeClass.Name} = new {attributeData.AttributeClass.Name}();");
        AspectProviderUtils.RecursiveGenerate(ref sourceBuilder, methodName, methodArgs, methodReturnType, methodModifiers, attributeDataIter, isAsync);
        sourceBuilder.AppendLine($@"            _{attributeData.AttributeClass.Name}.OnMethodExit();");
    }
}