using System;

namespace UniAOP {
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodValidationAspectAttribute : Attribute, IAspectAttribute {
        public abstract bool ValidateMethod();
    }
}