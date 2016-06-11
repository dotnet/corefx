Using MSBuild to build .NET Core projects
=========================================

The .NET Core tooling is going to [move from project.json to MSBuild based projects](https://blogs.msdn.microsoft.com/dotnet/2016/05/23/changes-to-project-json/).
We expect the first version of the .NET Core tools that use MSBuild to ship along with Visual Studio "15".  However, it is possible to use MSBuild for .NET Core
projects today, and this page shows how.

We recommend that most people targeting .NET Core today use the default tooling experience with project.json.  This is because we haven't yet added support to MSBuild
for a lot of the benefits that project.json has, because a lot of the ASP.NET based tooling will not work with MSBuild today, and because when we do release .NET Core
tooling which uses MSBuild, it will be able to automatically convert from project.json to MSBuild based projects.

You may want to consider using MSBuild to target .NET Core for existing projects that already use MSBuild that you want to port to .NET Core, or if you are using
MSBuild's extensibility in your build for scenarios that are not well supported for project.json projects.

Prerequisites
=============
- [Visual Studio 2015 Update 2 or higher](https://www.visualstudio.com/downloads/download-visual-studio-vs)
- [.NET Core tools for Visual Studio](https://www.visualstudio.com/downloads/download-visual-studio-vs)

Coretemplate
============
A good option to use to create an MSBuild based .NET Core project is to start with [coretemplate](https://github.com/mellinoe/coretemplate). This is a basic project which brings
in some MSBuild targets for .NET Core.

Starting from scratch
=====================
It is also possible to start by creating a project in Visual Studio and modify it to target .NET Core.  The instructions below show the minimal steps to get this working.
In contrast to coretemplate, a project created this way:

- Won't include configurations for targeting Linux and Mac OS
- Will use the CoreRun instead of the CoreConsole host.  This means that you will need to run the app via `CoreRun MyApp.exe` instead of just `MyApp.exe`.
To use the CoreConsole host, you need to reference the `Microsoft.NETCore.ConsoleHost` package instead of `Microsoft.NETCore.TestHost`, and after building rename `MyApp.exe`
to `MyApp.dll` and rename `CoreConsole.exe` to `MyApp.exe`.  Coretemplate does this in a [common targets file](https://github.com/mellinoe/corebuild/blob/master/coreconsole.targets).
- Visual Studio may ask you to enable developer mode for Windows 10 when you open the project



Starting from a portable library
- New library, target .NET 4.6 and .NET Core 1.0
- Update project.json

For an EXE
- Remove ProjecttypeGuids
- Set BaseNuGetRuntimeIdentifier to win10
- After targets import, set AutoUnifyAssemblyReferences property to true
- Change OutputType to Exe
- Change configuration to target x64 instead of AnyCPU
- Add debugging support
