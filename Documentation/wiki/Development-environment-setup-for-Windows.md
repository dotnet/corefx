This page describes the necessary steps for setting up your development environment on Windows in order to be able to make contributions to the repository.

## Required Software

1. **Visual Studio** must be installed. Supported versions:
    * [Visual Studio 2019](https://www.visualstudio.com/downloads/) (Community, Professional, Enterprise).  The Community version is completely free.
    * [Visual Studio 2017](https://www.visualstudio.com/downloads/) (Community, Professional, Enterprise).  The Community version is completely free.
    * [Visual Studio 2015](https://www.visualstudio.com/vs/older-downloads/) (Community, Professional, Enterprise).  The Community version is completely free.
2. **[CMake](https://cmake.org/)** must be installed from [the CMake download page](https://cmake.org/download/#latest) and added to your path.
   If you have an outdated cmake installed you will get error messages of the form 
     > CMake error : Could not create named generator Visual Studio 15 2017 Win64
 
   In that case uninstall cmake and then install the latest version.

### Visual Studio 2019 RC

Note: You must be using at least [CMake 3.14.0-rc3](https://cmake.org/download/) in order to use VS 2019.

### Visual Studio 2017

Note: If you have both VS 2017 and 2015 installed, you need to copy the `DIA SDK` directory from VS 2015 installation into VS 2017 (VS installer bug). The build script will detect the situation and tell you what to do.

#### Visual Studio 2017 - 'Workloads' based install

The following are the minimum requirements:
  * .NET desktop development
    * .NET Framework 4-4.6 Development Tools
  * Desktop development with C++
    * VC++ 2017 v141 Toolset (x86, x64)
    * Windows 10 SDK or Windows 8.1 SDK
  * .NET Core cross-platform development
    * All Required components

Estimated size: 10GB

#### Visual Studio 2017 - 'Individual components' based install

The following are the minimum requirements:
  * .NET
    * .NET Framework 4.6 targeting pack
    * .NET Portable Library targeting pack
  * Code tools
    * Static analysis tools
  * Compilers, build tools, and runtimes
    * C# and Visual Basic Roslyn compilers
    * MSBuild
    * VC++ 2017 version 15.8 v14.15 latest v141 tools
  * Development activities
    * Visual Studio C++ core features
  * SDKs, libraries, and frameworks
    * Windows 10 SDK (latest version) or Windows 8.1 SDK

Estimated size: 6GB

### Visual Studio 2015

* [Visual Studio 2015 Update 1](https://www.visualstudio.com/en-us/news/vs2015-update1-vs.aspx) is required.
* You must select `Programming Languages | Visual C++ | Common Tools for Visual C++ 2015` while installing VS 2015 (or modify your install to include it).
