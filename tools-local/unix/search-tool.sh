#!/usr/bin/env bash

usage()
{
    echo ""
    echo "usage: $0 <tool-name> <repository-root> <override-scripts-folder-path> <strict-tool-version-match>"
    echo "tool-name                         Name of the tool to search."
    echo "repository-root                   Path to repository root."
    echo "override-scripts-folder-path      If a path is specified, then scripts from the specified folder will be invoked."
    echo "                                  Otherwise, the default scripts located within the repository will be invoked."
    echo "strict-tool-version-match         If equals to \"strict\", then search will ensure that the version of the tool searched is the declared version."
    echo "                                  Otherwise, search will attempt to find a version of the tool, which may not be the declared version."
    echo ""
    echo "Searches for the tool in the environment path, and a path specified for the tool in the .toolversions file."
    echo "If search is successful, then returns the path of the tool."
    echo ""
}

toolName="$1"
repoRoot="$2"
overrideScriptsPath="$3"
strictToolVersionMatch="$4"

scriptPath="$(cd "$(dirname "$0")"; pwd -P)"
. "$scriptPath/tool-helper.sh"

exit_if_arg_count_mismatch "$#" "4" "$(usage)"
exit_if_arg_empty "$toolName" "tool-name" "$(usage)"
exit_if_invalid_path "$repoRoot" "repository-root" "$(usage)"
repoRoot="$(cd "$repoRoot"; pwd -P)"
exit_if_invalid_folder "$overrideScriptsFolderPath" "override-scripts-folder-path" "$(usage)" "ignoreEmptyValue"

search_environment()
{
    log_message "$repoRoot" "Searching for $toolName in environment path."
    hash "$toolName" 2>/dev/null

    if [ $? -ne 0 ]; then
        return
    fi

    toolPath="$(which $toolName)"
    validate_tool_path "$toolPath" "$toolName" "$strictToolVersionMatch" "$repoRoot" "$overrideScriptsPath"

    if [ $? -eq 0 ]; then
        echo "$toolPath"
        exit
    fi
}

search_cache()
{
    log_message "$repoRoot" "Searching for $toolName in the local tools cache."
    toolPath="$(get_local_search_path "$toolName" "$repoRoot")" || fail "$repoRoot" "$toolPath"
    validate_tool_path "$toolPath" "$toolName" "$strictToolVersionMatch" "$repoRoot" "$overrideScriptsPath"

    if [ $? -eq 0 ]; then
        echo "$toolPath"
        exit
    fi
}

search_environment

search_cache

exit 1
