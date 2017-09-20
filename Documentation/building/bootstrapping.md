To build .NET Core by yourself and bootstrap your application with it, follow these steps. 

## Setup
1. Clone the CoreClr and CoreFx repositories locally onto your machine.

```
git clone https://github.com/dotnet/coreclr.git
git clone https://github.com/dotnet/corefx.git
```
2. Follow the instructions to install the prerequisites.
- CoreClr: https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/developer-guide.md
- CoreFx: https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md

## Building
If you're on Windows use the Visual Studio Developer Command Prompt which is needed for building and later for using csc.
In this example we are building in Release mode (e.g. for Benchmarking) and skipping tests.
```
coreclr\build -Release -skiptests
corefx\build -release
```

## Application
We will build a sample application which outputs `Hello World!` to the console.
1. Create an application directory.
```
mkdir core-demo
mkdir core-demo\runtime
mkdir core-demo\ref
```

2. Save the following C# code to a file called Program.cs in your application folder.
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

## Bundle

1. Copy all required assemblies from CoreClr and CoreFx into your application folder.
- runtime: implementation assemblies needed for executing the application
- ref: reference assemblies which we are compiling against

```
copy coreclr\bin\Product\Windows_NT.x64.Release\clrjit.dll core-demo\runtime
copy coreclr\bin\Product\Windows_NT.x64.Release\CoreRun.exe core-demo\runtime
copy coreclr\bin\Product\Windows_NT.x64.Release\coreclr.dll core-demo\runtime
copy coreclr\bin\Product\Windows_NT.x64.Release\System.Private.CoreLib.dll core-demo\runtime

copy corefx\bin\Windows_NT.AnyCPU.Release\System.Console\netcoreapp\System.Console.dll core-demo\runtime
copy corefx\bin\Windows_NT.AnyCPU.Release\System.Runtime\netcoreapp\System.Runtime.dll core-demo\runtime
copy corefx\bin\Windows_NT.AnyCPU.Release\System.Runtime.Extensions\netcoreapp\System.Runtime.Extensions.dll core-demo\runtime
copy corefx\bin\Windows_NT.AnyCPU.Release\System.Runtime.InteropServices\netcoreapp\System.Runtime.InteropServices.dll core-demo\runtime
copy corefx\bin\Windows_NT.AnyCPU.Release\System.Text.Encoding.Extensions\netcoreapp\System.Text.Encoding.Extensions.dll core-demo\runtime
copy corefx\bin\Windows_NT.AnyCPU.Release\System.Threading\netcoreapp\System.Threading.dll core-demo\runtime

copy corefx\bin\ref\System.Runtime\4.2.1.0\netcoreapp\System.Runtime.dll core-demo\ref
copy corefx\bin\ref\System.Runtime.Extensions\4.2.1.0\netcoreapp\System.Runtime.Extensions.dll core-demo\ref
copy corefx\bin\ref\System.Console\4.1.1.0\netcoreapp\System.Console.dll core-demo\ref
```

2. Compile your application code with the C# Compiler against the required reference assemblies.
```
cd core-demo
csc /nostdlib /noconfig /r:ref\System.Runtime.dll /r:ref\System.Runtime.Extensions.dll /r:ref\System.Console.dll /out:runtime\Program.exe Program.cs
```

## Execution
```
cd runtime
corerun Program.exe
```

> Hello World!
