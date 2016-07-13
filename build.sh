#!/usr/bin/env bash

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__allArgs="$@"

#if $__buildnative; then
#    echo "$__scriptpath/build-native.sh $__allArgs"
#    "$__scriptpath/build-native.sh" $__allArgs
#    exit $?
#fi

echo "$__scriptpath/build-managed.sh $__allArgs"
"$__scriptpath/build-managed.sh" $__allArgs
exit $?

