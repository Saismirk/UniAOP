using System;
using UnityEngine;

namespace UniAOP.Example {
    public class TryCatchAspect : ExceptionAspectAttribute {
        public override void OnException(Exception e) => Debug.LogException(e);
    }
}