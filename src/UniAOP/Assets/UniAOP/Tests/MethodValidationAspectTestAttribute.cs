using NUnit.Framework;

namespace UniAOP.Tests {
    internal class MethodValidationAspectTestAttribute : MethodValidationAspectAttribute {
        public override bool ValidateMethod() {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(0));
            AspectTests.MethodCalls++;
            return AspectTests.MethodCondition;
        }
    }
}