# Using nightly builds of .NET Core 2.0

This document provides the steps necessary to consume a nightly build of .NET Core 2.0 runtime and SDK.

Please note that these steps are likely to change as we're simplifying this experience. Make sure to consult this document often.

## Install prerequisites

1. Acquire the latest nightly .NET Core SDK 2.0

- [Win 64-bit Latest Zip](https://dotnetcli.azureedge.net/dotnet/Sdk/master/dotnet-dev-win-x64.latest.zip) [Installer](https://dotnetcli.azureedge.net/dotnet/Sdk/master/dotnet-dev-win-x64.latest.exe)
- [macOS 64-bit Latest Tar](https://dotnetcli.azureedge.net/dotnet/Sdk/master/dotnet-dev-osx-x64.latest.tar.gz) [Installer](https://dotnetcli.azureedge.net/dotnet/Sdk/master/dotnet-dev-osx-x64.latest.pkg)
- [Others](https://github.com/dotnet/cli/blob/master/README.md#installers-and-binaries)

To setup the SDK, download the zip, extract it somewhere, and add the root folder to your path. Alternatively, always fully qualify the path to `dotnet` in the root of this folder for all instructions in this document.

Note: The installer will put `dotnet` globally in your path, which you might not want for dogfooding daily toolsets.

After setting up `dotnet` you can verify you are using the newer version  with `dotnet --info`. The version should be greater than 2.0.0-*.

## Setup the project

1. Create a new project
    - Create a new folder for your app
    - `cd` into it and run `dotnet new console`

2. Restore packages so that you're ready to play:

```
$ dotnet restore
```

## Consume the new build

Edit your `Program.cs` to consume the new APIs, for example:

```csharp
using System;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        WebUtility.HtmlDecode("&amp;", Console.Out);
        Console.WriteLine();
        Console.WriteLine("Hello World!");
    }
}
```

Run the bits:

```
$ dotnet run
```

Rinse and repeat!

## Advanced Scenario - Using a nightly build of Microsoft.NETCore.App

If you use the above instructions, your application will run against the same .NET Core 2.0 runtime that comes with the latest SDK. That works fine to get up and running quickly. However, there are times when you need to use a nightly build of Microsoft.NETCore.App which hasn't made its way into the SDK yet. To enable this, there are two options you can take.

### Option 1: Framework-dependent

This is the default case for applications - running against an installed .NET Core runtime.

0. You still need to install the prerequisite .NET Core SDK from above.
1. Also, install the specific .NET Core runtime you require:
    - https://github.com/dotnet/core-setup#daily-builds
    - Remember the version number you picked, you'll need it below.
2. Modify your `.csproj` to reference the nightly build of Microsoft.NETCore.App.

```xml
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <RuntimeFrameworkVersion>2.0.0-beta-xyz-00</RuntimeFrameworkVersion> <!-- pick nightly build -->
  </PropertyGroup>
```

```
$ dotnet restore
$ dotnet run
```

### Option 2: Self-contained

In this case, the .NET Core runtime will be published alongside your application.

0. You still need to install the prerequisite .NET Core SDK from above.
1. Modify your `.csproj` to reference the nightly build of Microsoft.NETCore.App *and*
make it self-contained.

```xml
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <RuntimeFrameworkVersion>2.0.0-beta-xyz-00</RuntimeFrameworkVersion> <!-- pick nightly build -->
    <RuntimeIdentifier>win7-x64</RuntimeIdentifier> <!-- make self-contained -->
  </PropertyGroup>
```

```
$ dotnet restore
$ dotnet publish
$ bin\Debug\netcoreapp2.0\win7-x64\publish\App.exe
```

Note #1: There is a [bug](https://github.com/dotnet/sdk/issues/791) with `dotnet run` and self-contained applications. If you `dotnet run` and see an error `The library 'hostpolicy.dll' is required to execute the application...`, you've hit this bug.

Note #2: On non-Windows platforms, self-contained applications aren't runnable by default. You will see an error "Permission denied" when running the application. This is because of a [breaking change in the .NET Core runtime](https://github.com/dotnet/corefx/issues/15516) between 1.0 and 2.0. Either this breaking change needs to be fixed, or [NuGet will have to workaround the change](https://github.com/NuGet/Home/issues/4424).

To workaround this issue, run `chmod u+x bin/Debug/netcoreapp2.0/RID/publish/App` before executing your application.

## Using your local CoreFX build

**If you choose Method 1 below, skip the "Self-contained" step where you modify the `.csproj`.**

To use your locally-built CoreFX packages, you will need a self-contained application and so you will need to follow the "Self-contained" steps from above. Once you successfully restore, build, and publish your self-contained application, configure it to use your CoreFX build using either of the two methods below.

### Method 1: Manually replace the stock framework assemblies with your build

This is the easiest way to get started using local builds. However, you must remember to repeat this every time you run `dotnet publish`.

Simply copy all files from

```
<corefx root>/bin/runtime/netcoreapp-<OS>-<configuration>-<architecture>
```

to

```
<application root>/bin/<configuration>/<framework>/<runtime>/publish
```

### Method 2: Reference the NuGet package produced by the CoreFX build

You only need to follow these steps once. Subsequent runs of `dotnet publish` should publish the assemblies from your CoreFX build automatically.

#### 1 - Get the version number of the CoreFX package you built.

Look for a package named `Microsoft.Private.CoreFx.NETCoreApp.<version>.nupkg` under `corefx\bin\packages\Debug` (or Release, if you built a release version of CoreFX).

Once you find the version number (for this example, assume it is `4.4.0-beta-25102-0`) you need to add the following line to your `.csproj`:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Private.CoreFx.NETCoreApp" Version="4.4.0-beta-25102-0" />
  </ItemGroup>
```

#### 2 - Add your `bin` directory to the NuGet feed list

By default, the dogfooding `dotnet` SDK will create a `NuGet.config` file next to your project. If it doesn't, you can create one. Your config file will need a source for your local CoreFX package directory, as well as a reference to our nightly dotnet-core feed on MyGet:

```xml
<configuration>
  <packageSources>
    <add key="local coreclr" value="D:\git\corefx\bin\packages\Debug" />
    <add key="dotnet-core" value="https://dotnet.myget.org/F/dotnet-core/api/v3/index.json" />
  </packageSources>
</configuration>

```
Obviously, **you need to update the path in the XML to point to your build's output directory**.

On Windows, you also have the alternative of modifying the `NuGet.config` at `%HOMEPATH%\AppData\Roaming\NuGet\NuGet.config` (on Linux, `~/.nuget/NuGet/NuGet.config`) for the new location. This will allow your new runtime to be used on any `dotnet restore` run by the current user. Alternatively, you can skip creating this file, and pass the path to your package directory using the `-s SOURCE` qualifer on the `dotnet restore` command below. The important part is that you have somehow told the tools where to find your new package.

Once have made these modifications, you will need to re-run the restore and publish:

```
dotnet restore
dotnet publish
```

Now your publication directory should contain your locally-built CoreFX binaries.

#### 3 - Consuming updated packages

One possible problem with the technique above is that NuGet assumes that distinct builds have distinct version numbers. Thus, if you modify the source and create a new NuGet package, you must give it a new version number and use that in your application's project. Otherwise the `dotnet.exe` tool will assume that the existing version is fine, and you won't get the updated bits. This is what the minor build number is all about. By default it is 0, but you can give it a value by setting the `BuildNumberMinor` environment variable:

```bat
set BuildNumberMinor=3
```

before packaging. You should see this number show up in the version number (e.g. 4.4.0-beta-25102-03).

As an alternative, you can delete the existing copy of the package from the NuGet cache. For example, on Windows (on Linux substitute `~/` for `%HOMEPATH%`) you could delete

```bat
%HOMEPATH%\.nuget\packages\Microsoft.Private.CoreFx.NETCoreApp\4.4.0-beta-25102-0
```

which should make things work (but is fragile, confirm with file timestamps that you are getting the version you expect).

### Consuming individual library packages

The instructions above were only about updates to the binaries that are part of Microsoft.NETCore.App. If you want to test a package for library that ships in its own NuGet package, follow the same steps as above, but instead add a package reference to the individual library package from your `bin\packages\Debug` folder.

## Consuming non-NetStandard assets in a .NET Core 2.0 application

Currently if you reference a NuGet package that does not have a NETStandard asset in your .NET Core 2.0 application, you will hit package incompatibility errors when trying to restore packages. You can resolve this issue by adding `PackageTargetFallback` property (MSBuild equivalent of `imports` from project.json) to your `.csproj`:

```xml
  <PackageTargetFallback>$(PackageTargetFallback);net45</PackageTargetFallback>
```

Note that this can only fix the problem if the package is actually compatible with netcoreapp2.0 (meaning it does not use types/APIs that are not available in netcoreapp2.0).

For final release, we are considering modifying NuGet behavior to automatically consume the non-netstandard asset if there is no netstandard available.

## Creating a .NET Core 2.0 console application from Visual Studio 2017

**File > New > Project > Console App (.NET Core)**

By default, Visual Studio creates a netcoreapp1.1 application. After installing the prerequisites mentioned above, you will need to modify your `.csproj` to target netcoreapp2.0 to reference the nightly build of Microsoft.NETCore.App.

```xml
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework> <!-- this line -->
    <RuntimeFrameworkVersion>2.0.0-beta-xyz-00</RuntimeFrameworkVersion> <!-- this line -->
  </PropertyGroup>
```

In a future update to Visual Studio, it will no longer be necessary to make this edit.

## Finding specific builds

The URL scheme for the runtime is as follows:

```
https://dotnetcli.azureedge.net/dotnet/master/Installers/$version$/dotnet-$os$-$arch$.$version$.exe
https://dotnetcli.azureedge.net/dotnet/master/Installers/2.0.0-preview1-001915-00/dotnet-win-x64.2.0.0-preview1-001915-00.exe
```

The URL scheme for the SDK & CLI is as follows:

```
https://dotnetcli.azureedge.net/dotnet/Sdk/$version$/dotnet-dev-$os$-$arch.$version$.exe
https://dotnetcli.azureedge.net/dotnet/Sdk/2.0.0-preview1-005791/dotnet-dev-win-x86.2.0.0-preview1-005791.exe
```
