#!/usr/bin/env bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__init_tools_log=$__scriptpath/init-tools.log
__PACKAGES_DIR=$__scriptpath/packages
__TOOLRUNTIME_DIR=$__scriptpath/Tools
__DOTNET_PATH=$__TOOLRUNTIME_DIR/dotnetcli
__DOTNET_CMD=$__DOTNET_PATH/dotnet
if [ -z "$__BUILDTOOLS_SOURCE" ]; then __BUILDTOOLS_SOURCE=https://dotnet.myget.org/F/dotnet-buildtools/api/v3/index.json; fi
__BUILD_TOOLS_PACKAGE_VERSION=$(cat $__scriptpath/BuildToolsVersion.txt)
__DOTNET_TOOLS_VERSION=$(cat $__scriptpath/DotnetCLIVersion.txt)
__BUILD_TOOLS_PATH=$__PACKAGES_DIR/Microsoft.DotNet.BuildTools/$__BUILD_TOOLS_PACKAGE_VERSION/lib
__PROJECT_JSON_PATH=$__TOOLRUNTIME_DIR/$__BUILD_TOOLS_PACKAGE_VERSION
__PROJECT_JSON_FILE=$__PROJECT_JSON_PATH/project.json
__PROJECT_JSON_CONTENTS="{ \"dependencies\": { \"Microsoft.DotNet.BuildTools\": \"$__BUILD_TOOLS_PACKAGE_VERSION\" }, \"frameworks\": { \"netcoreapp1.0\": { } } }"
__INIT_TOOLS_DONE_MARKER=$__PROJECT_JSON_PATH/done

display_error_message()
{
    echo "Please check the detailed log that follows." 1>&2
    cat "$__init_tools_log" 1>&2
}

if [ ! -e $__INIT_TOOLS_DONE_MARKER ]; then
    if [ -e $__TOOLRUNTIME_DIR ]; then rm -rf -- $__TOOLRUNTIME_DIR; fi
    echo "Running: $__scriptpath/init-tools.sh" > $__init_tools_log
    if [ ! -e $__DOTNET_PATH ]; then
        if [ -z "$__DOTNET_PKG" ]; then
            __PKG_ARCH=x64
            
            OSName=$(uname -s)
            case $OSName in
                Darwin)
                    __PKG_RID=osx
                    OS=OSX
                    ulimit -n 2048
                    ;;

                Linux)
                    __PKG_RID=linux
                    OS=Linux
                    
                    if [ -e /etc/os-release ]; then
                        source /etc/os-release
                        __DISTRO_NAME=$ID.$VERSION_ID
                        if  [ "$__DISTRO_NAME" == 'ubuntu.16.04' ] ||
                            [ "$__DISTRO_NAME" == 'ubuntu.16.10' ] ||
                            [ "$__DISTRO_NAME" == 'ubuntu.18.04' ] ||
                            [ "$__DISTRO_NAME" == 'fedora.23' ] ||
                            [ "$__DISTRO_NAME" == 'fedora.24' ] ||
                            [ "$__DISTRO_NAME" == 'fedora.27' ] ||
                            [ "$__DISTRO_NAME" == 'opensuse.13.2' ] ||
                            [ "$__DISTRO_NAME" == 'opensuse.42.1' ]; then
                            __PKG_RID=$__DISTRO_NAME
                        elif [ "$__DISTRO_NAME" == 'ubuntu.14.04' ]; then
                            __PKG_RID='ubuntu'
                        elif [ "$(echo "$__DISTRO_NAME" | grep -cim1 centos)" -eq 1 ]; then
                            __PKG_RID='centos'
                        fi
                    fi
                    
                    ;;
                *)
                echo "Unsupported OS '$OSName' detected. Downloading linux-$__PKG_ARCH tools."
                    OS=Linux
                    __PKG_RID=linux
                    ;;
            esac
            
            __DOTNET_PKG=dotnet-dev-$__PKG_RID-$__PKG_ARCH
        fi

        echo "Installing dotnet cli..."
        __DOTNET_LOCATION="https://dotnetcli.blob.core.windows.net/dotnet/Sdk/${__DOTNET_TOOLS_VERSION}/${__DOTNET_PKG}.${__DOTNET_TOOLS_VERSION}.tar.gz"
        # curl has HTTPS CA trust-issues less often than wget, so lets try that first.
        echo "Installing '${__DOTNET_LOCATION}' to '$__DOTNET_PATH/dotnet.tar'" >> $__init_tools_log
        which curl > /dev/null 2> /dev/null
        if [ $? -ne 0 ]; then
            mkdir -p "$__DOTNET_PATH"
            wget -q -O $__DOTNET_PATH/dotnet.tar ${__DOTNET_LOCATION}
        else
            curl --retry 10 -sSL --create-dirs -o $__DOTNET_PATH/dotnet.tar ${__DOTNET_LOCATION}
        fi
        cd $__DOTNET_PATH
        tar -xf $__DOTNET_PATH/dotnet.tar

        cd $__scriptpath
    fi

    if [ ! -d "$__PROJECT_JSON_PATH" ]; then mkdir "$__PROJECT_JSON_PATH"; fi
    echo $__PROJECT_JSON_CONTENTS > "$__PROJECT_JSON_FILE"

    if [ ! -e $__BUILD_TOOLS_PATH ]; then
        echo "Restoring BuildTools version $__BUILD_TOOLS_PACKAGE_VERSION..."
        echo "Running: $__DOTNET_CMD restore \"$__PROJECT_JSON_FILE\" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE" >> $__init_tools_log
        $__DOTNET_CMD restore "$__PROJECT_JSON_FILE" --no-cache --packages $__PACKAGES_DIR --source $__BUILDTOOLS_SOURCE >> $__init_tools_log
        if [ ! -e "$__BUILD_TOOLS_PATH/init-tools.sh" ]; then
            echo "ERROR: Could not restore build tools correctly." 1>&2
            display_error_message
        fi
    fi

    echo "Initializing BuildTools..."
    echo "Running: $__BUILD_TOOLS_PATH/init-tools.sh $__scriptpath $__DOTNET_CMD $__TOOLRUNTIME_DIR" >> $__init_tools_log
    $__BUILD_TOOLS_PATH/init-tools.sh $__scriptpath $__DOTNET_CMD $__TOOLRUNTIME_DIR >> $__init_tools_log
    if [ "$?" != "0" ]; then
        echo "ERROR: Could not restore build tools correctly." 1>&2
        display_error_message
        exit 1
    fi

    cp Tools-Override/* Tools/
    if [ $? -ne 0 ]; then
        echo [ERROR] Failed to copy Tools-Override.
        exit $?
    fi
    
    # Replace the binaries restored by the tool runtime script with the portable binaries
    if [ "$__PKG_RID" == "linux" ]; then
         cp -r $__DOTNET_PATH/shared/Microsoft.NETCore.App/*/* $__TOOLRUNTIME_DIR
    fi

    touch $__INIT_TOOLS_DONE_MARKER
    echo "Done initializing tools."
else
    echo "Tools are already initialized"
fi
