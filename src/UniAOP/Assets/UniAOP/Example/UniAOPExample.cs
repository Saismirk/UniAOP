using System;
using System.Threading.Tasks;
using UniAOP;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniAOP.Example {
    public partial class UniAOPExample : MonoBehaviour {

        public async void Start() {
            await _SomeMethod();
            _UnvalidatedMethod();
        }

        [LoggedAspect]
        private static async Task<string> SomeMethod() {
            await Task.Yield();
            return _UnvalidatedMethod();
        }
        
        [LoggedAspect]
        [WithValidationAspect]
        [TryCatchAspect]
        private static string UnvalidatedMethod() {
            Debug.Log("UnvalidatedMethod");
            return "Hello World!";
        }
    }
    
    
    public class LoggedAspect : MethodBoundaryAspectAttribute  {
        public override void OnMethodEnter() {
            Debug.Log("Enter Method");
        }
        
        public override void OnMethodExit() {
            Debug.Log("Exit Method");
        }
    }
    
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

    public class TryCatchAspect : ExceptionAspectAttribute {
        public override void OnException(Exception e) => Debug.LogException(e);
    }
}