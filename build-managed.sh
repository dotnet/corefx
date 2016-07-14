#!/usr/bin/env bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)

__UnprocessedBuildArgs=
__ServerGC=0

while :; do
    if [ $# -le 0 ]; then
        break
    fi

    lowerI="$(echo $1 | awk '{print tolower($0)}')"
    case $lowerI in
        useservergc)
            __ServerGC=1
            ;;
        *)
            __UnprocessedBuildArgs="$__UnprocessedBuildArgs $1"
    esac

    shift
done

export CORECLR_SERVER_GC="$__ServerGC"

$__scriptpath/run.sh build-managed -binclashUnix -os -target-os -osversion $*
exit $?
