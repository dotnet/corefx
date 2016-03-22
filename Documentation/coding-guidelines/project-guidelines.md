#Library Project guidelines
Library projects should use the fllowing directory layout.
```
src\<Library Name>\src - Contains the source code for the library.
src\<Library Name>\ref - Contains any reference assembly projects for the library
src\<Library Name>\pkg - Contains package projects for the library.
src\<Library Name>\tests - Contains the test code for a library
```
In the src directory for a library there should be only **one** `.csproj` file that contains any information necessary to build the library in various configurartions (see [Configurations](#project-configuration-conventions)). The src directory should also contain exactly **one** `.builds` file which contains all the valid configurations that a library should be built for (see [#.builds file](#project-builds-file)). In some cases there might be a Facade subdirectory under src with a .csproj (see [Facades projects](#facades-projects)).

In the ref directory for the library there should be at most **one** `.csproj` that contains the latest API for the reference assembly for the library.  If a library cannot support all supported targets using the latest API it should use `.depproj` projects for each significant historical version in order to download and redistribute the historical reference assembly in the latest package.  If a library is a pure portable library with a single implementation it need not use a reference assembly at all. (see [Reference assembly projects](#reference-assembly-projects)).

In the pkg directory for the library there should be only **one** `.pkgproj` for the primary package for the library.  If the library has platform-specific implementations those should be split into platform specific projects in a subfolder for each platform. (see [Package projects](./package-projects.md))

##Build Pivots
Below is a list of all the various options we pivot the project builds on.

- **Architecture:** x86, x64, ARM, ARM64
- **Flavor:** Debug, Release
- **OS:** Windows_NT, Linux, OSX, FreeBSD
- **Platform Runtimes:** NetFx (aka CLR/Desktop), CoreCLR, NetNative (aka MRT/AOT)
- **Target Frameworks:** NetFx (aka Desktop), .NETCore (aka Store/UWP), DNXCore (aka ASP.NET vNext), netstandard(aka dotnet/Portable)
- **Version:** Potentially multiple versions at the same time.

##Full Repo build pass
**Build Parameters:** *Flavor, Architecture*<BR/>
For each combination of build parameters there should be a full build pass over the entire repo.

##Project build pass
**Optional Build Parameters:** *OS, Platform Runtime, Target Framework, Version*<BR/>
These are optional build parameters for specific projects and don't require a full build pass but instead should be scoped to individual projects within the most appropriate full build pass.

#Project configuration conventions
For each unique configuration needed for a given library project a configuration property group should be added to the project so it can be selected and built in VS and also clearly identify the various configurations.<BR/>
`<PropertyGroup Condition="'$(Configuration)|$(Platform)' == '$(<OSGroup>_<TargetGroup>_<ConfigurationGroup>)|$(Platform)'">`

- `$(Platform) -> AnyCPU* | x86 | x64 | ARM | ARM64`
- `$(Configuration) -> $(OSGroup)_$(TargetGroup)_$(ConfigurationGroup)`
- `$(OSGroup) -> [Empty]/AnyOS* | Windows | Linux | OSX | FreeBSD`
- `$(TargetGroup) -> [Empty]* | <PackageTargetFramework> | <PackageTargetRuntime> | <Version> | <PackageTargetFramework><PackageTargetRuntime>`
 - `$(PackageTargetFramework) -> netstandard1.x/(currently dotnet5.x) | netcore50 | net46 | dnxcore50`
 - `$(PackageTargetRuntime) -> aot`
 - For more information on various targets see also [.NET Platform Standard](https://github.com/dotnet/corefx/blob/master/Documentation/architecture/net-platform-standard.md)
- `$(ConfigurationGroup) -> Debug* | Release`
<BR/>`*` -> *default values*

####*Examples*
Project configurations for a pure IL library project which targets the defaults.
```
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Release|AnyCPU'" />
```
Project configurations with a unique implementation for each OS
```
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'FreeBSD_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'FreeBSD_Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Linux_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Linux_Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'OSX_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'OSX_Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Windows_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Windows_Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Windows_netcore50_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Windows_netcore50_Release|AnyCPU'" />
```
Project configurations that are unique for a few different target frameworks and runtimes
```
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcore50aot_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcore50aot_Release|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcore50_Debug|AnyCPU'" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcore50_Release|AnyCPU'" />
```
## Project .builds file
To drive the Project build pass we have a `.builds` project file that will multiplex the various optional build parameters we have for all the various configurtions within a given build pass.

####*Examples*

Project configurations for pure IL library project
```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.props))\dir.props" />
  <ItemGroup>
    <Project Include="System.Reflection.Metadata.csproj" />
  </ItemGroup>
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.traversal.targets))\dir.traversal.targets" />
</Project>
```
Project configurations with a unique implementation for each OS
```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.props))\dir.props" />
  <ItemGroup>
    <Project Include="System.IO.FileSystem.csproj">
      <OSGroup>FreeBSD</OSGroup>
    </Project>
    <Project Include="System.IO.FileSystem.csproj">
      <OSGroup>Linux</OSGroup>
    </Project>
    <Project Include="System.IO.FileSystem.csproj">
      <OSGroup>OSX</OSGroup>
    </Project>
    <Project Include="System.IO.FileSystem.csproj">
      <OSGroup>Windows_NT</OSGroup>
    </Project>
    <Project Include="facade\System.IO.FileSystem.csproj">
      <TargetGroup>net46</TargetGroup>
    </Project>
    <Project Include="System.IO.FileSystem.csproj">
      <OSGroup>Windows_NT</OSGroup>
      <TargetGroup>netcore50</TargetGroup>
    </Project>
  </ItemGroup>
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.traversal.targets))\dir.traversal.targets" />
</Project>

```
Project configurations that are unique for a few different target frameworks and runtimes
```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.props))\dir.props" />
  <ItemGroup>
    <Project Include="System.Linq.Expressions.csproj" />
    <Project Include="Facade\System.Linq.Expressions.csproj">
      <TargetGroup>net46</TargetGroup>
    </Project>
    <Project Include="System.Linq.Expressions.csproj">
      <OSGroup>Windows_NT</OSGroup>
      <TargetGroup>netcore50</TargetGroup>
    </Project>
    <Project Include="System.Linq.Expressions.csproj">
      <OSGroup>Windows_NT</OSGroup>
      <TargetGroup>netcore50aot</TargetGroup>
    </Project>
  </ItemGroup>
  <Import Project="$([MSBuild]::GetDirectoryNameOfFileAbove($(MSBuildThisFileDirectory), dir.traversal.targets))\dir.traversal.targets" />
</Project>
```

##Facades projects
Facade projects are unique in that they don't have any code and instead are generated by finding a contract reference assembly with the matching identity and generating type forwards for all the types to where they live in the implementation assemblies (aka facade seeds). There are also partial facades which contain some type forwards as well as some code definitions. Ideally all the various build configurations would be contained in the one csproj file per library but given the unique nature of facades and the fact they are usually a complete fork of everything in the project file it is recommended to create a matching csproj under a Facade directory (<library>\src\Facade\<library>.csproj) and reference that from your .builds file, as in the System.Linq.Expressions example.

TODO: Fill in more information about the required properties for creatng a facade project.

# Conventions for forked code
While our goal is to have the exact code for every configuration there is always reasons why that is not realistic so we need to have a set of conventions for dealing with places where we fork code. In order of preference, here are the strategies we employ:

1. Using different code files with partial classes to implement individual methods different on different configurations
2. Using entirely different code files for cases were the entire class (or perhaps static class) needs to be unique in a given configuration.
3. Using `#ifdef`'s directly in a shared code file.

In general we perfer different code files over `#ifdef`'s because it forces us to better factor the code which leads to easier maintenance over time. 

## Code file naming conventions
Each source file should use the following guidelines
- The source code file should contain only one class. The only exception is small supporting structs, enums, nested classes, or delegates that only apply to the class can also be contained in the source file. 
- The source code file should be named `<class>.cs` and should be placed in a directory structure that matches its namespace relative to its project directory. Ex. `System\IO\Stream.cs` 
- Larger nested classes should be factored out into their own source files using a partial class and the file name should be `<class>.<nested class>.cs`.
- Classes that are forked based on configuration should have file names `<class>.<configuration>.cs`. 
 - Where `<configuration>` is one of `$(OSGroup)`, `$(TargetGroup)`, `$(ConfigurationGroup)`, or `$(Platform)`, matching exactly by case to ensure consistency.
- Classes that are forked based on a feature set should have file names `<class>.<feature>.cs`.
 - Where `<feature>`is the name of something that causes a fork in code that isn't a configuration such as WinRT support. Ex. `FileSystemInfo.WinRT.cs`.
 
## Define naming convention
As mentioned in [Conventions for forked code](conventions-for-forked-code) `#ifdef`ing the code is the last resort as it makes code harder to maintain overtime. If we do need to use `#ifdef`'s we should use the following conventions:
- Defines based on conventions should be one of `$(OSGroup)`, `$(TargetGroup)`, `$(ConfigurationGroup)`, or `$(Platform)`, matching exactly by case to ensure consistency. 
 - Examples: `<DefineConstants>$(DefineConstants),netcore50</DefineContants>`
- Defines based on convention should match the pattern `FEATURE_<feature name>`. These can unique to a given library project or potentially shared (via name) across multiple projects.

# Reference assembly projects
Reference assemblies are required for any library that has more than one implementation or uses a facade.  A reference assembly is a surface-area-only assembly that represents the public API of the library.  To generate a reference assembly source file you can use the [GenAPI tool](https://www.nuget.org/packages/Microsoft.DotNet.BuildTools.GenAPI).

When adding API to a library it is sometimes impossible to support the new API on all the platforms where the library is supported today.  We strive to support new APIs everywhere, but sometimes this is not possible.  For example: string is part of the core library so a new property on string must exist on the core library type.  We cannot ship a new copy of string to the desktop due to limitations in binding rules for the desktop.  Instead of dropping support for desktop we include an older version of the reference assembly that represents the smaller surface area for that version of desktop.  This is achieved by adding a deployment project (`.depproj`) to deploy the old reference assembly from the old package.
