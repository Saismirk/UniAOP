using NUnit.Framework;

namespace UniAOP.Tests {
    internal class MethodExitAspectTestAttribute : MethodExitAspectAttribute {
        public override void OnMethodExit() {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(0));
            AspectTests.MethodCalls++;
        }
    }
}