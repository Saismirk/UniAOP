using System.Text;
using Microsoft.CodeAnalysis;

namespace UniAOP;

public interface IAspectProvider {
    void Generate(ref StringBuilder sourceBuilder,
        string methodName,
        string methodArgs,
        string methodReturnType,
        string methodModifiers,
        AttributeData attributeData,
        bool isAsync);
}