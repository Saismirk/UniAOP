using NUnit.Framework;

namespace UniAOP.Tests {
    internal class MethodEnterAspectTestAttribute : MethodEnterAspectAttribute {
        public override void OnMethodEnter() {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(0));
            AspectTests.MethodCalls++;
        }
    }
}