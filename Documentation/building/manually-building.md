# Build and run your application with csc and corerun

This tutorial describes how to build and run an application that targets self-compiled .NET Core binaries. We won't be using Visual Studio,  the .NET Core SDK Host (`dotnet.exe`), nor a C# project file (`*.csproj`). Follow these steps to quickly validate changes you made in the product e.g. by running benchmarks or tests on it. Don't consider using these instructions for production scenarios. At the time of writing required tools like `csc` are not accessible on Unix/MacOS machines, therefore make sure to __use a Windows environment__.

Use the `Developer Command Prompt for VS 2017` which is necessary for building .NET Core and later for using `csc`.

## Compile corefx with self-compiled coreclr binaries
If you have made changes to coreclr you'll also want to build it and pass its binaries to corefx.
```
d:\git\coreclr\build -release
d:\git\corefx\build -release -- /p:CoreCLROverridePath=d:\git\coreclr\bin\Product\Windows_NT.x64.Release\
```

## Compile corefx with pre-compiled coreclr binaries
If you haven't made any changes to coreclr you're fine with just building corefx. This will automatically pick pre-compiled coreclr binaries from MyGet.
```
d:\git\corefx\build -release
```

## Create and prepare your application
We will build a sample application which outputs `Hello World!` to the console.

1. Create an application directory.
```
mkdir d:\git\core-demo
```

2. Save the following C# code to a file called `Program.cs` in your application folder.
```csharp
using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello World!");
    }
}
```

3. Copy the just built corefx assemblies into your application directory. When using Visual Studio or the .NET Core SDK Host (`dotnet.exe`) you usually compile against *reference assemblies*. For simplicity we'll compile against the same assembly set that we'll use during run time. 
```
xcopy d:\git\corefx\bin\testhost\netcoreapp-Windows_NT-Release-x64\shared\Microsoft.NETCore.App\9.9.9 d:\git\core-demo\runtime
```

You won't need all the assemblies but copying the entire directory makes it easier if you want to reference additional ones. For __running__ our Hello World application the following assemblies are needed:

- CoreClr assemblies: `clrjit.dll`, `CoreRun.exe`, `coreclr.dll`, `System.Private.CoreLib.dll`
- CoreFx assemblies: `System.Runtime.dll`, `System.Runtime.Extensions.dll`, `System.Runtime.InteropServices.dll`, `System.Text.Encoding.Extensions.dll`, `System.Threading.dll`

## Compile your application
Compile your application with the C# Compiler (`csc`) against the copied assemblies.
```
csc /nostdlib /noconfig /r:runtime\System.Runtime.dll /r:runtime\System.Runtime.Extensions.dll /r:runtime\System.Console.dll /out:runtime\Program.dll Program.cs
```

## Run your application
`Corerun.exe` is part of the coreclr binaries and is best described as the host of your .NET Core application.
```
cd runtime
.\corerun Program.dll
```

> Hello World!
