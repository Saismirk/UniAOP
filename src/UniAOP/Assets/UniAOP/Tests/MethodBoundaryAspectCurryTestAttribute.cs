using NUnit.Framework;

namespace UniAOP.Tests {
    internal class MethodBoundaryAspectCurryTestAttribute : MethodBoundaryAspectAttribute {
        public override void OnMethodEnter() {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(1));
            AspectTests.MethodCalls++;
        }

        public override void OnMethodExit() {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(2));
            AspectTests.MethodCalls++;
        }
    }
}