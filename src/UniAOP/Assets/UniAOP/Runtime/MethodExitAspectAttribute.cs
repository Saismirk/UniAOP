using System;

namespace UniAOP {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodExitAspectAttribute : Attribute, IAspectAttribute {
        public abstract void OnMethodExit();
    }
}