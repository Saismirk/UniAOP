using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public class MethodValidationAsyncAspectProvider : IAspectProvider {
    public void Generate(ref StringBuilder sourceBuilder,
        string methodName,
        string methodArgs,
        string methodReturnType,
        string methodModifiers,
        AttributeData attributeData,
        bool isAsync) {
        if (attributeData?.AttributeClass is null) return;
        sourceBuilder.AppendLine($@"
        {methodModifiers} {methodReturnType} {methodName}{attributeData.AttributeClass.Name.Replace("Aspect", "")} ({methodArgs}) {{ 
            var attribute = new {attributeData.AttributeClass.Name}();
            if (await attribute.ValidateMethod({methodArgs}) == false) {{
                return;
            }};

            {(isAsync ? "await " : "")}{methodName}({methodArgs});
        }}
");
    }
}