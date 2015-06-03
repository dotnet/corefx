#!/bin/bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__packageroot=$__scriptpath/packages
__sourceroot=$__scriptpath/src
__nugetpath=$__packageroot/NuGet.exe
__nugetconfig=$__sourceroot/NuGet.Config
__msbuildpackageid="Microsoft.Build.Mono.Debug"
__msbuildpackageversion="14.1.0.0-prerelease"
__msbuildpath=$__packageroot/$__msbuildpackageid.$__msbuildpackageversion/lib/MSBuild.exe

if [ $(uname) == "Linux" ]; then
    __monoroot=/usr
else
    __monoroot=/Library/Frameworks/Mono.framework/Versions/Current
fi

__referenceassemblyroot=$__monoroot/lib/mono/xbuild-frameworks


__monoversion=$(mono --version | grep "version 4.[1-9]")

if [ $? -ne 0 ]; then
    echo "Mono 4.1 or later is required to build corefx. Please see https://github.com/dotnet/corefx/blob/master/Documentation/linux-instructions.md for more details."
    exit 1
fi

if [ ! -e "$__referenceassemblyroot/.NETPortable" ]; then
    echo "PCL reference assemblies not found. Please see https://github.com/dotnet/corefx/blob/master/Documentation/linux-instructions.md for more details."
    exit 1
fi

__buildproj=$__scriptpath/build.proj
__buildlog=$__scriptpath/msbuild.log

# Pull NuGet.exe down if we don't have it already
if [ ! -e "$__nugetpath" ]; then
    which curl wget > /dev/null 2> /dev/null
    if [ $? -ne 0 -a $? -ne 1 ]; then
        echo "cURL or wget is required to build corefx. Please see https://github.com/dotnet/corefx/blob/master/Documentation/linux-instructions.md for more details."
        exit 1
    fi
    echo "Restoring NuGet.exe..."

    which wget > /dev/null 2> /dev/null
    if [ $? -ne 0 ]; then
       curl -sSL --create-dirs -o $__nugetpath https://api.nuget.org/downloads/nuget.exe
    else
       mkdir -p $__packageroot
       wget -q -O $__nugetpath https://api.nuget.org/downloads/nuget.exe
    fi

    if [ $? -ne 0 ]; then
        echo "Failed to restore NuGet.exe."
        exit 1
    fi
fi

# Grab the MSBuild package if we don't have it already
if [ ! -e "$__msbuildpath" ]; then
    echo "Restoring MSBuild..."
    mono "$__nugetpath" install $__msbuildpackageid -Version $__msbuildpackageversion -ConfigFile "$__nugetconfig" -OutputDirectory "$__packageroot"
    if [ $? -ne 0 ]; then
        echo "Failed to restore MSBuild."
        exit 1
    fi
fi


if [ $(uname) == "Linux" ]; then
    __osgroup=Linux
else
    __osgroup=OSX
fi

MONO29679=1 ReferenceAssemblyRoot=$__referenceassemblyroot mono $__msbuildpath "$__buildproj" /nologo /verbosity:minimal "/fileloggerparameters:Verbosity=diag;LogFile=$__buildlog" /t:Build /p:OSGroup=$__osgroup /p:UseRoslynCompiler=true /p:COMPUTERNAME=$(hostname) /p:USERNAME=$(id -un) "$@"
BUILDERRORLEVEL=$?

echo

# Pull the build summary from the log file
tail -n 4 "$__buildlog"
echo Build Exit Code = $BUILDERRORLEVEL

exit $BUILDERRORLEVEL
