#!/usr/bin/env bash

if [ $# == 0 ]; then
    __args=-p
fi

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
# Building CoreFx.Tools before calling build-managed.sh to workaround an Assembly loading bug caused by the new cli host.
"$__scriptpath/Tools/msbuild.sh" "$__scriptpath/src/Tools/CoreFx.Tools/CoreFx.Tools.csproj" /v:m /m
if [ $? -ne 0 ];then
   exit 1
fi
$__scriptpath/run.sh sync $__args $*
exit $?
