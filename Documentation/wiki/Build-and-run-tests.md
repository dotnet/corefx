This page describes the process for compiling the repository and running tests.

### Quick Reference

#### Typical workflow

1. `build` - Builds the product, not tests.
    * **Note:** You need to **build from root** after each major update of sources.
2. `build -buildtests` builds test projects. 
3. `build -test` - **runs** tests, test projects need to be built beforehand.
4. `build -restore -build -buildtests -test` - **builds** all source and test projects, then **runs** tests.


#### Tips & tricks

* `build -clean` - Cleans up the binary output and optionally the working directory (-all). Useful if you get into messy state.
* Move [NuGet cache](https://github.com/dotnet/corefx/wiki/Build-and-run-tests#nuget-cache-customization) location out of C drive.
* `build -restore` - Pulls down all external dependencies (so you can do the rest offline).


# Build and Run tests

After each major update of sources, you need to **build from root**, otherwise you may encounter weird build errors. On rare occasion, clean is needed.
Build from root has to happen from command line (we do not have all-repo solution for Visual Studio).

When you iterate on code changes, you can rebuild just sub-projects, using incremental build, in both command line and Visual Studio.


## Build from root

From a (non-admin) **Command Prompt window** (You SHOULD find this in your Start Menu under Visual Studio 2015/2017 after they are installed, instead of a Command window from `Run` or `Windows appendage`, also make sure you have PowerShell v3 or later installed), and then run:

1. `build` script (.cmd/.sh) - Builds the shipping libraries in corefx (3+ min). Does not build tests.
2. `build & build -buildtests -test` - Builds everything and **runs** the corefx tests (4+ min).
3. *[Optional]* `build -clean` - Cleans up the binary output and optionally the working directory (-all).
    * Useful when you get your local clone into weird state.
4. *[Optional]* `build -restore` - Pulls down external dependencies needed to build (i.e. build tools, xunit, coreclr, etc).
    * Useful to prep for offline work. It will run automatically also during `build`, if you don't run it explicitly.
    * Expect 2GB+ in clone and 1GB in NuGet cache.

### NuGet Cache Customization

NuGet cache defaults to C drive and can grow quickly to GBs. You can move it to another disk by editing [`%APPDATA%\NuGet\NuGet.Config`](https://docs.microsoft.com/en-us/nuget/schema/nuget-config-file#config-section) file:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <config>
        <add key="globalPackagesFolder" value="D:\code\packages" />
```

To avoid downloading GBs again, just move the directory to its new place when you modify the config.


## Incremental Build or Run tests

To rebuild a particular library, run `msbuild` from its 'src' subdirectory. To run tests once, run `msbuild` from its 'tests' subdirectory. On Windows msbuild should be in PATH when running from Developer command prompt.

Example:

```
cd src\System.Console\src
dotnet msbuild

cd src\System.Console\tests
dotnet msbuild /t:rebuildandtest  
```


## Build, Run and Debug tests using Visual Studio

1. Open solution file (.sln) for a specific library (e.g. `src\System.Collections\System.Collections.sln`).
2. Right-click test project and select `Set as StartUp Project`.
3. Ctrl+F5 (Run) to run tests.
4. F5 (Debug) to debug tests (breakpoints work in test and source code).
    * Trick: Many negative tests throw exceptions caught by xunit test framework. VS treats them as unhandled and breaks on them. To disable this behavior, uncheck `Enable Just My Code` in `Tools | Options | Debugging | General`.


### Run and Debug single test in Visual Studio

While you can see and navigate to tests, you cannot currently run them from the Visual Studio Test Explorer. To run or debug a single test from within Visual Studio, you need to temporarily add command line filter options and run/debug the project as described above. These options should be added at the end of the `Application arguments:` in the test project properties `Debug` tab.

The existing arguments will look something like this: `$(RunArguments) -wait -parallel none`. Here are two methods to run specific tests:

1. Specify the test method name.
2. Add traits to the test and specify the trait name.

The test method filter is specified using `-method FullNameToMethod`. For example: `-method *.ObjectTests.ReadSimpleClass` or `-method System.Text.Json.Tests.ObjectTests.ReadSimpleClass`.

Trait attributes are specified using `-trait MyTrait=MyTraitValue`. Tests need the trait attribute applied, e.g. `[Trait("MyTrait", "MyTraitValue")]`.

Example:
```c#
[Fact]
[Trait("MyTrait", "MyTraitValue")]
public void MyTest()
{
   // ...
}
```

These changes need to be undone before making commits and PRs.

### Run and Debug single test in Command Line

To quickly run or debug a single test from the command line, pass the XunitMethodName option to msbuild, specifying the full method name (including namespace).
For example:
```
dotnet msbuild /t:rebuildandtest /p:XunitMethodName=System.MemoryTests.ReadOnlyMemoryTests.ToStringChar_Empty
```
It is not necessary to use `/t:rebuildandtest` to force tests to re-run. If you want to skip the build and simply run tests again, use `/t:test /p:ForceRunTests=true`.

The `BuildAndTest`, `RebuildAndTest` and `Test` targets are only available in the test csproj files. Make sure to run this command either on the `test` subdirectory, or invoking it directly on the test csproj file.

Or, you can eliminate the MSBuild wrapper and run the XUnit command directly. The console output shows you how, for example,
```
---- start 21:30:12.43 ===============  To repro directly: =====================================================
pushd C:\git\corefx\bin\tests\System.Text.RegularExpressions.Tests\netcoreapp-Windows_NT-Debug-x64\
call C:\git\corefx\bin\testhost\netcoreapp-Windows_NT-Debug-x64\\dotnet.exe xunit.console.netcore.exe System.Text.RegularExpressions.Tests.dll  -xml testResults.xml -notrait Benchmark=true -notrait category=nonnetcoreapptests -notrait category=nonwindowstests  -notrait category=OuterLoop -notrait category=failing
popd
===========================================================================================================
```


## Outerloop tests

When tests are unavoidably resource intensive (they take significant time to complete due to the nature of what is being tested) and are not expected to be broken easily, consider using the `[Outerloop]` attribute for the test. With this attribute, tests are executed in a dedicated CI loop that runs outside of the default CI loops which get created when you submit a PR.

To run outerloop tests locally you need to use the `-OuterLoop` with build (if using msbuild directly: `/p:TestScope=all`).
To run outerloop tests in CI you need to mention dotnet-bot and tell him which tests you want to run. See `@dotnet-bot help` for the exact loop names.


## Advanced Build options

- `-framework netcoreapp|netfx|uap` - Target specific Framework. Default is `netcoreapp`.
    * `netcoreapp` - .NET Core
    * `netfx` - "full" .NET Framework (Windows-only)
    * `uap` - UWP (Universal Windows Platform) (Windows-only)
- `-os Windows_NT|Linux|OSX` - Target specific OS (`Windows_NT`=any Windows OS, `OSX`=macOS). Default is current OS.
- `-c Debug` or `-c Release` - Debug vs. Release build. Default is `-c debug`.
- `-arch x64|x86|arm|arm64` - Target specific processor architecture. Default is `x64`.
