using UnityEngine;

namespace UniAOP.Example {
    public class WithValidationAspect : MethodValidationAspectAttribute {
        public override bool ValidateMethod() {
            Debug.Log("Validating Method...");
            if (Random.Range(0, 10) > 5) {
                Debug.Log("Invalid Method");
                return false;
            }
            
            Debug.Log("Valid Method");
            return true;
        }
    }
}