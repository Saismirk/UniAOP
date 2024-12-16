using UnityEngine;

namespace UniAOP.Example {
    public class LoggedAspect : MethodBoundaryAspectAttribute  {
        public override void OnMethodEnter() {
            Debug.Log("Enter Method");
        }
        
        public override void OnMethodExit() {
            Debug.Log("Exit Method");
        }
    }
}