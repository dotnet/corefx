#!/usr/bin/env bash

usage()
{
    echo ""
    echo "usage: $0 cmake <repository-root> <override-scripts-folder-path> <tool-path>"
    echo "repository-root                   Path to repository root."
    echo "override-scripts-folder-path      This argument is ignored."
    echo "tool-path                         Path to CMake executable."
    echo ""
    echo "If successful, then returns the version number of CMake executable."
    echo ""
}

toolName="$1"
repoRoot="$2"
toolPath="$4"

scriptPath="$(cd "$(dirname "$0")/.."; pwd -P)"
. "$scriptPath/tool-helper.sh"

exit_if_arg_count_mismatch "$#" "4" "$(usage)"
exit_if_invalid_path "$toolPath" "tool-path" "$(usage)"
[ "$toolName" == "cmake" ] || fail "$repoRoot" "Second argument should be cmake." "$(usage)"
exit_if_invalid_path "$repoRoot" "repository-root" "$(usage)"

# Extract version number. For example, 3.6.0 in text below:
# cmake version 3.6.0
toolVersion="$("$toolPath" -version | grep -o '[0-9]\+\..*')"

[ ! -z "$toolVersion" ] || fail "$repoRoot" "Unable to determine the version of CMake at $toolPath."
echo "$toolVersion"
