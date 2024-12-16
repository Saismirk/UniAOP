using System;

namespace UniAOP {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodBoundaryAspectAttribute : Attribute, IAspectAttribute {
        public abstract void OnMethodEnter();
        public abstract void OnMethodExit();
    }
}