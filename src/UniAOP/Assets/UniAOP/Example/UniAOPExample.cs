using System.Threading.Tasks;
using UniAOP;
using UnityEngine;

namespace UniAOP.Example {
    public partial class UniAOPExample : MonoBehaviour {

        public async void Start() {
            await SomeMethod();
            UnvalidatedMethod();
        }

        [LoggedAspect]
        private static async Task<string> _SomeMethod() {
            await Task.Yield();
            return UnvalidatedMethod();
        }
        
        [LoggedAspect]
        [WithValidationAspect]
        [TryCatchAspect]
        private static string _UnvalidatedMethod() {
            Debug.Log("UnvalidatedMethod");
            return "Hello World!";
        }
    }
}