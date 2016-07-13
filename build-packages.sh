#!/usr/bin/env bash

RuntimeOS=ubuntu.14.04
# Use uname to determine what the OS is.
OSName=$(uname -s)
case $OSName in
    Darwin)
        # Darwin version can be three sets of digits (e.g. 10.10.3), we want just the first one
        DarwinVersion=$(sw_vers -productVersion | awk 'match($0, /[0-9]+/) { print substr($0, RSTART, RLENGTH) }')
        RuntimeOS=osx.$DarwinVersion
        ;;

    FreeBSD|NetBSD)
        # TODO this doesn't seem correct
        RuntimeOS=osx.10
        ;;

    Linux)
        if [ ! -e /etc/os-release ]; then
            echo "Cannot determine Linux distribution, assuming Ubuntu 14.04"
        else
            source /etc/os-release
            # for some distros we only need the version major number
            VersionMajor=$(echo $VERSION_ID | awk 'match($0, /[0-9]+/) { print substr($0, RSTART, RLENGTH) }')
            if [ "$ID" == "rhel" ]; then
                RuntimeOS=$ID.$VersionMajor
            else
                RuntimeOS=$ID.$VERSION_ID
            fi
        fi
        ;;

    *)
        echo "Unsupported OS '$OSName' detected. Configuring as if for Ubuntu."
        ;;
esac

working_tree_root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
$working_tree_root/run.sh build-managed -packages -binclashUnix $* "/p:FilterToOSGroup=$RuntimeOS"
exit $?
