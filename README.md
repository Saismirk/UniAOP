UniAOP
===

**UniAOP** is a framework for Unity Engine that allows the use of Aspect Oriented Programming (AOP) in Unity scripts.

- Based on [Aspect Oriented Programming](https://en.wikipedia.org/wiki/Aspect-oriented_programming) (AOP)
- Uses Roslyn Source Generators to create the AOP code at compile time
- Currying support
- Compatible with async methods

Getting Started
---
Install via [OpenUPM](https://openupm.com/packages/com.sai.uniaop/) or unitypackage in [UniAOP/Releases](https://github.com/Saismirk/UniAOP/releases)

Usage
---
Four types of AOPs are provided:
- [MethodBoundaryAspect](#methodboundaryaspect)
- [MethodEnterAspect](#methodenteraspect)
- [MethodExitAspect](#methodexitaspect)
- [MethodExceptionAspect](#methodexceptionaspect)
- [MethodValidationAspect](#methodvalidationaspect)

MethodBoundaryAspect
---
Used to call abstract `OnMethodEnter` and `OnMethodExit` at the start and end of a method.
```csharp
using UniAOP;

public class LoggingAspect : MethodBoundaryAspectAttribute {
    public override void OnMethodEnter() {
        Debug.Log("Enter Method");
    }
        
    public override void OnMethodExit() {
        Debug.Log("Exit Method");
    }
}

public class MyClass {
    [LoggingAspect]
    public void MyMethod() {
        Debug.Log("My Method");
    }
}
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
Enter Method
My Method
Exit Method
```

MethodEnterAspect
---
Used to call abstract `OnMethodEnter` at the start of a method.
```csharp
using UniAOP;

public class LoggingAspect : MethodEnterAspectAttribute {
    public override void OnMethodEnter() {
        Debug.Log("Enter Method");
    }
}

public class MyClass {
    [LoggingAspect]
    public void MyMethod() {
        Debug.Log("My Method");
    }
}  
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
Enter Method
My Method
```

MethodExitAspect
---
Used to call abstract `OnMethodExit` at the end of a method.
```csharp
using UniAOP;

public class LoggingAspect : MethodExitAspectAttribute {
    public override void OnMethodExit() {
        Debug.Log("Exit Method");
    }
}

public class MyClass {
    [LoggingAspect]
    public void MyMethod() {
        Debug.Log("My Method");
    }
}  
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
My Method
Exit Method
```

MethodExceptionAspect
---
Used to add a try-catch block to a method. The catch block will call the `OnMethodException` method.
```csharp
using UniAOP;

public class ExceptionAspect : MethodExceptionAspectAttribute {
    public override void OnMethodException(Exception exception) {
        Debug.LogException(exception);
    }
}

public class MyClass {
    [ExceptionAspect]
    public void MyMethod() {
        throw new Exception("This is an exception");
    }
}  
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
System.Exception: This is an exception
```

MethodValidationAspect
---
Used to add a validation block to a method. The validation is based on the result of the `OnMethodValidation` method.
```csharp
using UniAOP;

public class ValidationAspect : MethodValidationAspectAttribute {
    public override bool OnMethodValidation() {
        return true;
    }
}

public class MyClass {
    [ValidationAspect]
    public void MyMethod() {
        Debug.Log("My Method");
    }
}  
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
My Method
```

Currying Support
---
Multiple Aspect attributes can be applied to a single method.
```csharp
using UniAOP;

public class LoggingAspect : MethodBoundaryAspectAttribute {
    public override void OnMethodEnter() {
        Debug.Log("Enter Method");
    }
        
    public override void OnMethodExit() {
        Debug.Log("Exit Method");
    }
}

public class ValidationAspect : MethodValidationAspectAttribute {
    public override bool OnMethodValidation() {
        Debug.Log("Validation");
        return true;
    }
}

public class MyClass {
    [ValidationAspect]
    [LoggingAspect]
    public void MyMethod() {
        Debug.Log("My Method");
    }
}
```
Calling the generated method `_MyMethod()` will result in the following output to the Unity Console:
```
Validation
Enter Method
My Method
Exit Method
```

Limitations
---
**UniAOP** currently only works on `void` methods or `async` methods with no return values (`Task`/`UniTask`/`async void`/`UniTaskVoid`).