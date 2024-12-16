using System;
using NUnit.Framework;

namespace UniAOP.Tests {
    internal class ExceptionAspectTestAttribute : ExceptionAspectAttribute {
        public override void OnException(Exception e) {
            Assert.That(AspectTests.MethodCalls, Is.EqualTo(0));
            AspectTests.MethodCalls++;
        }
    }
}