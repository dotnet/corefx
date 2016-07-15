#!/usr/bin/env bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)

"$__scriptpath/build-native.sh" $*
if [ $? -ne 0 ]
    exit 1
fi

"$__scriptpath/build-managed.sh" $*
exit $?

