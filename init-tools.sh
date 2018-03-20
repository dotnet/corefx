#!/usr/bin/env bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__init_tools_log=$__scriptpath/init-tools.log
__PACKAGES_DIR=$__scriptpath/packages
__TOOLRUNTIME_DIR=$__scriptpath/Tools
__DOTNET_PATH=$__TOOLRUNTIME_DIR/dotnetcli
__DOTNET_CMD=$__DOTNET_PATH/dotnet
if [ -z "$__BUILDTOOLS_SOURCE" ]; then __BUILDTOOLS_SOURCE=https://dotnet.myget.org/F/dotnet-buildtools/api/v3/index.json; fi
export __BUILDTOOLS_USE_CSPROJ=true
__BUILD_TOOLS_PACKAGE_VERSION=$(cat $__scriptpath/BuildToolsVersion.txt)
__DOTNET_TOOLS_VERSION=$(cat $__scriptpath/DotnetCLIVersion.txt)
__BUILD_TOOLS_PATH=$__PACKAGES_DIR/microsoft.dotnet.buildtools/$__BUILD_TOOLS_PACKAGE_VERSION/lib
__INIT_TOOLS_RESTORE_PROJECT=$__scriptpath/init-tools.msbuild
__BUILD_TOOLS_SEMAPHORE=$__TOOLRUNTIME_DIR/$__BUILD_TOOLS_PACKAGE_VERSION/init-tools.complete

if [ -z "$__DOTNET_PKG" ]; then
    if [ "$(uname -m | grep "i[3456]86")" = "i686" ]; then
        echo "Warning: build not supported on 32 bit Unix"
    fi
    OSName=$(uname -s)
    case $OSName in
        Darwin)
            OS=OSX
            __DOTNET_PKG=dotnet-dev-osx-x64
            ulimit -n 2048
            # Format x.y.z as single integer with three digits for each part
            VERSION=`sw_vers -productVersion| sed -e 's/\./ /g' | xargs printf "%03d%03d%03d"`
            if [ "$VERSION" -lt 010012000 ]; then
                echo error: macOS version `sw_vers -productVersion` is too old. 10.12 is needed as minimum.
                exit 1
            fi
            ;;

        Linux)
            __DOTNET_PKG=dotnet-dev-linux-x64
            OS=Linux
            ;;

        *)
            echo "Unsupported OS '$OSName' detected. Downloading linux-x64 tools."
            OS=Linux
            __DOTNET_PKG=dotnet-dev-linux-x64
            ;;
  esac
fi

display_error_message()
{
    echo "Please check the detailed log that follows." 1>&2
    cat "$__init_tools_log" 1>&2
}

if [ ! -e $__INIT_TOOLS_DONE_MARKER ]; then
    __PATCH_CLI_NUGET_FRAMEWORKS=0

if [ -d "$DotNetBuildToolsDir" ]; then
    echo "Using tools from '$DotNetBuildToolsDir'."
    ln -s "$DotNetBuildToolsDir" "$__TOOLRUNTIME_DIR"

    if [ ! -e "$__DOTNET_CMD" ]; then
        echo "ERROR: Ensure that $DotNetBuildToolsDir contains the .NET Core SDK at $__DOTNET_PATH"
        exit 1
    fi

    echo "Done initializing tools."
    mkdir -p "$(dirname "$__BUILD_TOOLS_SEMAPHORE")" && touch $__BUILD_TOOLS_SEMAPHORE
    return #return instead of exit because this script is inlined in other scripts which we don't want to exit
fi

        if [ -n "$DOTNET_TOOLSET_DIR" ] && [ -d "$DOTNET_TOOLSET_DIR/$__DOTNET_TOOLS_VERSION" ]; then
            echo "Copying $DOTNET_TOOLSET_DIR/$__DOTNET_TOOLS_VERSION to $__DOTNET_PATH" >> $__init_tools_log
            cp -r $DOTNET_TOOLSET_DIR/$__DOTNET_TOOLS_VERSION/* $__DOTNET_PATH
        elif [ -n "$DOTNET_TOOL_DIR" ] && [ -d "$DOTNET_TOOL_DIR" ]; then
            echo "Copying $DOTNET_TOOL_DIR to $__DOTNET_PATH" >> $__init_tools_log
            cp -r $DOTNET_TOOL_DIR/* $__DOTNET_PATH
        else
            echo "Installing dotnet cli..."
            __DOTNET_LOCATION="https://dotnetcli.azureedge.net/dotnet/Sdk/${__DOTNET_TOOLS_VERSION}/${__DOTNET_PKG}.${__DOTNET_TOOLS_VERSION}.tar.gz"
            # curl has HTTPS CA trust-issues less often than wget, so lets try that first.
            echo "Installing '${__DOTNET_LOCATION}' to '$__DOTNET_PATH/dotnet.tar'" >> $__init_tools_log
            if command -v curl > /dev/null; then
                curl --retry 10 -sSL --create-dirs -o $__DOTNET_PATH/dotnet.tar ${__DOTNET_LOCATION}
            else
                wget -q -O $__DOTNET_PATH/dotnet.tar ${__DOTNET_LOCATION}
            fi
            cd $__DOTNET_PATH
            tar -xf $__DOTNET_PATH/dotnet.tar

            cd $__scriptpath

            __PATCH_CLI_NUGET_FRAMEWORKS=1
        fi
    done

    return 0
}

    if [ -n "$BUILD_TOOLS_TOOLSET_DIR" ] && [ -d "$BUILD_TOOLS_TOOLSET_DIR/$__BUILD_TOOLS_PACKAGE_VERSION" ]; then
        echo "Copying $BUILD_TOOLS_TOOLSET_DIR/$__BUILD_TOOLS_PACKAGE_VERSION to $__TOOLRUNTIME_DIR" >> $__init_tools_log
        cp -r $BUILD_TOOLS_TOOLSET_DIR/$__BUILD_TOOLS_PACKAGE_VERSION/* $__TOOLRUNTIME_DIR
    elif [ -n "$BUILD_TOOLS_TOOL_DIR" ] && [ -d "$BUILD_TOOLS_TOOL_DIR" ]; then
        echo "Copying $BUILD_TOOLS_TOOL_DIR to $__TOOLRUNTIME_DIR" >> $__init_tools_log
        cp -r $BUILD_TOOLS_TOOL_DIR/* $__TOOLRUNTIME_DIR
    else
        if [ ! -e $__BUILD_TOOLS_PATH ]; then
            echo "Restoring BuildTools version $__BUILD_TOOLS_PACKAGE_VERSION..."
            echo "Running: $__DOTNET_CMD restore \"$__INIT_TOOLS_RESTORE_PROJECT\" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE /p:BuildToolsPackageVersion=$__BUILD_TOOLS_PACKAGE_VERSION" >> $__init_tools_log
            $__DOTNET_CMD restore "$__INIT_TOOLS_RESTORE_PROJECT" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE /p:BuildToolsPackageVersion=$__BUILD_TOOLS_PACKAGE_VERSION >> $__init_tools_log
            if [ ! -e "$__BUILD_TOOLS_PATH/init-tools.sh" ]; then
                echo "ERROR: Could not restore build tools correctly." 1>&2
                display_error_message
            fi
        fi

       __PKG_ARCH=x64

        OSName=$(uname -s)
        case $OSName in
            Darwin)
                OS=OSX
                __PKG_RID=osx
                ulimit -n 2048
                # Format x.y.z as single integer with three digits for each part
                VERSION=`sw_vers -productVersion| sed -e 's/\./ /g' | xargs printf "%03d%03d%03d"`
                if [ "$VERSION" -lt 010012000 ]; then
                    echo error: macOS version `sw_vers -productVersion` is too old. 10.12 is needed as minimum.
                    exit 1
                fi
                ;;

            Linux)
            __PKG_RID=linux
                OS=Linux

                if [ -e /etc/os-release ]; then
                    source /etc/os-release
                    if [[ $ID == "alpine" ]]; then
                        # remove the last version digit
                        VERSION_ID=${VERSION_ID%.*}
                        __PKG_RID=alpine.$VERSION_ID
                    fi
                elif [ -e /etc/redhat-release ]; then
                    redhatRelease=$(</etc/redhat-release)
                    if [[ $redhatRelease == "CentOS release 6."* || $redhatRelease == "Red Hat Enterprise Linux Server release 6."* ]]; then
                        __PKG_RID=rhel.6
                    fi
                fi

                ;;

            *)
                echo "Unsupported OS '$OSName' detected. Downloading linux-$__PKG_ARCH tools."
                OS=Linux
                __PKG_RID=linux
                ;;
      esac

      __DOTNET_PKG=dotnet-sdk-${__DOTNET_TOOLS_VERSION}-$__PKG_RID-$__PKG_ARCH
    fi
    mkdir -p "$__DOTNET_PATH"

    echo "Installing dotnet cli..."
    __DOTNET_LOCATION="https://dotnetcli.azureedge.net/dotnet/Sdk/${__DOTNET_TOOLS_VERSION}/${__DOTNET_PKG}.tar.gz"

        # Executables restored with .NET Core 2.0 do not have executable permission flags. https://github.com/NuGet/Home/issues/4424
        chmod +x $__BUILD_TOOLS_PATH/init-tools.sh
        $__BUILD_TOOLS_PATH/init-tools.sh $__scriptpath $__DOTNET_CMD $__TOOLRUNTIME_DIR >> $__init_tools_log
        if [ "$?" != "0" ]; then
            echo "ERROR: An error occurred when trying to initialize the tools." 1>&2
            display_error_message
            exit 1
        fi
        cd $__DOTNET_PATH
        tar -xf $__DOTNET_PATH/dotnet.tar
    }
    execute_with_retry install_dotnet_cli >> "$__init_tools_log" 2>&1

    cd $__scriptpath
fi

if [ ! -e $__BUILD_TOOLS_PATH ]; then
    echo "Restoring BuildTools version $__BUILD_TOOLS_PACKAGE_VERSION..."
    echo "Running: $__DOTNET_CMD restore \"$__INIT_TOOLS_RESTORE_PROJECT\" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE /p:BuildToolsPackageVersion=$__BUILD_TOOLS_PACKAGE_VERSION" >> $__init_tools_log
    $__DOTNET_CMD restore "$__INIT_TOOLS_RESTORE_PROJECT" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE /p:BuildToolsPackageVersion=$__BUILD_TOOLS_PACKAGE_VERSION >> $__init_tools_log
    if [ ! -e "$__BUILD_TOOLS_PATH/init-tools.sh" ]; then
        echo "ERROR: Could not restore build tools correctly." 1>&2
        display_error_message
    fi
fi

echo "Initializing BuildTools..."
echo "Running: $__BUILD_TOOLS_PATH/init-tools.sh $__scriptpath $__DOTNET_CMD $__TOOLRUNTIME_DIR" >> $__init_tools_log

# Executables restored with .NET Core 2.0 do not have executable permission flags. https://github.com/NuGet/Home/issues/4424
chmod +x $__BUILD_TOOLS_PATH/init-tools.sh
$__BUILD_TOOLS_PATH/init-tools.sh $__scriptpath $__DOTNET_CMD $__TOOLRUNTIME_DIR >> $__init_tools_log
if [ "$?" != "0" ]; then
    echo "ERROR: An error occurred when trying to initialize the tools." 1>&2
    display_error_message
    exit 1
fi

echo "Making all .sh files executable under Tools."
# Executables restored with .NET Core 2.0 do not have executable permission flags. https://github.com/NuGet/Home/issues/4424
ls $__scriptpath/Tools/*.sh | xargs chmod +x
ls $__scriptpath/Tools/scripts/docker/*.sh | xargs chmod +x

Tools/crossgen.sh $__scriptpath/Tools

mkdir -p "$(dirname "$__BUILD_TOOLS_SEMAPHORE")" && touch $__BUILD_TOOLS_SEMAPHORE

echo "Done initializing tools."

