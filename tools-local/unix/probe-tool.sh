#!/usr/bin/env bash

usage()
{
    echo ""
    echo "usage: $0 <tool-name> <repository-root> <override-scripts-folder-path> <strict-tool-version-match>"
    echo "tool-name                         Name of the tool to search and/or download."
    echo "repository-root                   Path to repository root."
    echo "override-scripts-folder-path      If a path is specified, then search and acquire scripts from the specified folder will be invoked."
    echo "                                  Otherwise, search will use the default search and acquire scripts located within the repository."
    echo "strict-tool-version-match         If equals to \"strict\", then search will ensure that the version of the tool searched is the declared version."
    echo "                                  Otherwise, search will attempt to find a version of the tool, which may not be the declared version."
    echo ""
    echo "Invokes an extension that calls the appropriate search and/or acquire scripts."
    echo "tool-name, repository-root, override-scripts-folder-path and strict-tool-version-match are passed on to the extension."
    echo ""
    echo "Example #1"
    echo "probe-tool.sh cmake \"/Users/dotnet/corefx\" "" """
    echo "Probes for CMake, not necessarily the declared version, using the default search and acquire scripts located within the repository."
    echo ""
    echo "Example #2"
    echo "probe-tool.sh cmake \"/Users/dotnet/corefx\" "" strict"
    echo "Probes for the declared version of CMake using the default search and acquire scripts located within the repository."
    echo ""
    echo "Example #3"
    echo "probe-tool.sh cmake \"/Users/dotnet/corefx\" \"/Users/dotnet/MyCustomScripts\" strict"
    echo "Probes for the declared version of CMake using the search and acquire scripts located in \"/Users/dotnet/MyCustomScripts\"."
    echo ""
}

toolName="$1"
repoRoot="$2"
overrideScriptsFolderPath="$3"
strictToolVersionMatch="$4"

scriptPath="$(cd "$(dirname "$0")"; pwd -P)"
. "$scriptPath/tool-helper.sh"

exit_if_arg_count_mismatch "$#" "4" "$(usage)"
exit_if_arg_empty "$toolName" "tool-name" "$(usage)"
exit_if_invalid_path "$repoRoot" "repository-root" "$(usage)"
repoRoot="$(cd "$repoRoot"; pwd -P)"
exit_if_invalid_folder "$overrideScriptsFolderPath" "override-scripts-folder-path" "$(usage)" "ignoreEmptyValue"

toolPath="$(invoke_extension "search-tool.sh" "$toolName" "$repoRoot" "$overrideScriptsFolderPath" "$strictToolVersionMatch")"

if [ $? -ne 0 ]; then
    toolPath="$(invoke_extension "acquire-tool.sh" "$toolName" "$repoRoot" "$overrideScriptsFolderPath")"
fi

echo "$toolPath"
