using System;

namespace UniAOP {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ExceptionAspectAttribute : Attribute, IAspectAttribute {
        public abstract void OnException(Exception exception);
    }
}