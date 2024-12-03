using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public class MethodBoundaryAspectProvider : IAspectProvider {
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
            attribute.OnMethodEnter();
            {(isAsync ? "await " : "")}{methodName}({methodArgs});
            attribute.OnMethodExit();
        }}
");
    }
}