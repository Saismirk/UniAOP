using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public class ExceptionAspectProvider : IAspectProvider {
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
            var _{attributeData.AttributeClass.Name} = new {attributeData.AttributeClass.Name}();
            try {{");
        AspectProviderUtils.RecursiveGenerate(ref sourceBuilder, methodName, methodArgs, methodReturnType, methodModifiers, attributeDataIter, isAsync, 1);
        sourceBuilder.AppendLine($@"            }}
            catch (System.Exception ex) {{
                _{attributeData.AttributeClass.Name}.OnException(ex);
            }}");
    }
}