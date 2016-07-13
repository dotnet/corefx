#!/usr/bin/env bash

usage()
{
    echo "Usage: $0 [BuildArch] [BuildType] [verbose] [platform] [skiptests] [useservergc]"
    echo "BuildArch can be: x64, x86, arm, arm-softfp, arm64"
    echo "BuildType can be: debug, release"
    echo "verbose - optional argument to enable verbose build output."
    echo "platform can be: FreeBSD, Linux, NetBSD, OSX, Windows"
    echo "skiptests - skip the tests in the './bin/*/*Tests/' subdirectory."
    exit 1
}

# Prepare the system for building
prepare_managed_build()
{
    # Run Init-Tools to restore BuildTools and ToolRuntime
    $__scriptpath/init-tools.sh
}

build_managed()
{
    __buildproj=$__scriptpath/build.proj
    __buildlog=$__scriptpath/msbuild.log
    __binclashlog=$__scriptpath/binclash.log
    __binclashloggerdll=$__scriptpath/Tools/Microsoft.DotNet.Build.Tasks.dll

    $__scriptpath/Tools/dotnetcli/dotnet $__scriptpath/Tools/MSBuild.exe "$__buildproj" /m /nologo /verbosity:minimal "/flp:Verbosity=normal;LogFile=$__buildlog" "/flp2:warningsonly;logfile=$__scriptpath/msbuild.wrn" "/flp3:errorsonly;logfile=$__scriptpath/msbuild.err" "/l:BinClashLogger,$__binclashloggerdll;LogFile=$__binclashlog" /p:ConfigurationGroup=$__BuildType /p:TargetOS=$__BuildOS /p:OSGroup=$__BuildOS /p:SkipTests=$__SkipTests /p:COMPUTERNAME=$(hostname) /p:USERNAME=$(id -un) /p:TestNugetRuntimeId=$__TestNugetRuntimeId $__UnprocessedBuildArgs
    exit $?
}

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__TestNugetRuntimeId=win7-x64

# Use uname to determine what the CPU is.
CPUName=$(uname -p)
# Some Linux platforms report unknown for platform, but the arch for machine.
if [ $CPUName == "unknown" ]; then
    CPUName=$(uname -m)
fi

case $CPUName in
    i686)
        __BuildArch=x86
        ;;

    x86_64)
        __BuildArch=x64
        ;;

    armv7l)
        __BuildArch=arm
        ;;

    aarch64)
        __BuildArch=arm64
        ;;

    *)
        echo "Unknown CPU $CPUName detected, configuring as if for x64"
        __BuildArch=x64
        ;;
esac

# Use uname to determine what the OS is.
OSName=$(uname -s)
case $OSName in
    Darwin)
        __HostOS=OSX
        __TestNugetRuntimeId=osx.10.10-x64
        ;;

    FreeBSD)
        __HostOS=FreeBSD
        # TODO: Add native version
        __TestNugetRuntimeId=osx.10.10-x64
        ;;

    Linux)
        __HostOS=Linux
        if [ ! -e /etc/os-release ]; then
            echo "Cannot determine Linux distribution, assuming Ubuntu 14.04"
            __TestNugetRuntimeId=ubuntu.14.04-x64
        else
            source /etc/os-release
            __TestNugetRuntimeId=$ID.$VERSION_ID-$__BuildArch
        fi
        ;;

    NetBSD)
        __HostOS=NetBSD
        # TODO: Add native version
        __TestNugetRuntimeId=osx.10.10-x64
        ;;

    *)
        echo "Unsupported OS '$OSName' detected. Configuring as if for Ubuntu."
        __HostOS=Linux
        __TestNugetRuntimeId=ubuntu.14.04-x64
        ;;
esac
__BuildOS=$__HostOS
__BuildType=Debug

BUILDERRORLEVEL=0

# Set the various build properties here so that CMake and MSBuild can pick them up
__UnprocessedBuildArgs=
__SkipTests=false
__ServerGC=0

while :; do
    if [ $# -le 0 ]; then
        break
    fi

    lowerI="$(echo $1 | awk '{print tolower($0)}')"
    case $lowerI in
        -\?|-h|--help)
            usage
            exit 1
            ;;
        x86)
            __BuildArch=x86
            ;;
        x64)
            __BuildArch=x64
            ;;
        arm)
            __BuildArch=arm
            ;;
        arm-softfp)
            __BuildArch=arm-softfp
            ;;
        arm64)
            __BuildArch=arm64
            ;;
        debug)
            __BuildType=Debug
            ;;
        release)
            __BuildType=Release
            __CMakeArgs=RELEASE
            ;;
        verbose)
            __VerboseBuild=1
            ;;
        freebsd)
            __BuildOS=FreeBSD
            __TestNugetRuntimeId=osx.10.10-x64
            ;;
        linux)
            __BuildOS=Linux
            # If the Host OS is also Linux, then use the RID of the host.
            # Otherwise, override it to Ubuntu by default.
            if [ "$__HostOS" != "Linux" ]; then
                __TestNugetRuntimeId=ubuntu.14.04-x64
            fi
            ;;
        netbsd)
            __BuildOS=NetBSD
            __TestNugetRuntimeId=osx.10.10-x64
            ;;
        osx)
            __BuildOS=OSX
            __TestNugetRuntimeId=osx.10.10-x64
            ;;
        windows)
            __BuildOS=Windows_NT
            __TestNugetRuntimeId=win7-x64
            ;;
        skiptests)
            __SkipTests=true
            ;;
        useservergc)
            __ServerGC=1
            ;;
        *)
            __UnprocessedBuildArgs="$__UnprocessedBuildArgs $1"
    esac

    shift
done

export CORECLR_SERVER_GC="$__ServerGC"

    # Prepare the system

    prepare_managed_build

    # Build the corefx native components.

    build_managed

    # Build complete
fi

