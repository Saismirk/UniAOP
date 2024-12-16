using System;

namespace UniAOP {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodEnterAspectAttribute : Attribute, IAspectAttribute {
        public abstract void OnMethodEnter();
    }
}
