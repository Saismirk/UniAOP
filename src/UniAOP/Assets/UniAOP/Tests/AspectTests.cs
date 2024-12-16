using System;
using NUnit.Framework;

namespace UniAOP.Tests {
    public partial class AspectTests {
        internal static int MethodCalls;
        internal static bool MethodCondition;

        [MethodBoundaryAspectTest]
        private void _MethodBoundaryAspectTestMethod() {
            Assert.That(MethodCalls, Is.EqualTo(1));
        }
        
        [MethodBoundaryAspectTest]
        private bool _MethodBoundaryAspectTestMethodReturn() {
            Assert.That(MethodCalls, Is.EqualTo(1));
            return true;
        }
        
        [MethodEnterAspectTest]
        private bool _MethodEnterAspectTestMethod() {
            Assert.That(MethodCalls, Is.EqualTo(1));
            return true;
        }
        
        [MethodExitAspectTest]
        private bool _MethodExitAspectTestMethod() {
            Assert.That(MethodCalls, Is.EqualTo(0));
            return true;
        }
        
        [ExceptionAspectTest]
        private bool _ExceptionAspectTestMethod() {
            throw new Exception();
        }
        
        [MethodValidationAspectTest]
        private bool _MethodValidationAspectTestMethod() {
            MethodCalls++;
            return true;
        }
        
        [MethodValidationAspectTest]
        [MethodBoundaryAspectCurryTest]
        private bool _MethodAspectCurryingTestMethod() {
            Assert.That(MethodCalls, Is.EqualTo(2));
            return true;
        }

        [Test]
        public void MethodBoundaryAspectTest() {
            MethodCalls = 0;
            MethodBoundaryAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(2));
        }
        
        [Test]
        public void MethodBoundaryReturnAspectTest() {
            MethodCalls = 0;
            var result = MethodBoundaryAspectTestMethodReturn();
            Assert.That(MethodCalls, Is.EqualTo(2));
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void MethodEnterAspectTest() {
            MethodCalls = 0;
            var result = MethodEnterAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(1));
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void MethodExitAspectTest() {
            MethodCalls = 0;
            var result = MethodExitAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(1));
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void ExceptionAspectTest() {
            MethodCalls = 0;
            var result = ExceptionAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(1));
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void MethodValidationAspectTest() {
            MethodCalls = 0;
            MethodCondition = true;
            var result = MethodValidationAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(2));
            Assert.That(result, Is.True);
            
            MethodCalls = 0;
            MethodCondition = false;
            result = MethodValidationAspectTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(1));
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void MethodValidationBoundaryAspectTest() {
            MethodCalls = 0;
            MethodCondition = true;
            MethodAspectCurryingTestMethod();
            Assert.That(MethodCalls, Is.EqualTo(3));
        }
    }
}