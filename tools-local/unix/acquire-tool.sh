#!/usr/bin/env bash

usage()
{
    echo ""
    echo "usage: $0 <tool-name> <repository-root> <override-scripts-folder-path>"
    echo "tool-name                         Name of the tool to download."
    echo "repository-root                   Path to repository root."
    echo "override-scripts-folder-path      If a path is specified, then scripts from the specified folder will be invoked."
    echo "                                  Otherwise, the default scripts located within the repository will be invoked."
    echo ""
    echo "Downloads the declared version of the specified tool from the corresponding URL specified in the .toolversions file."
    echo "If download succeeds, then returns the path to the executable."
    echo ""
}

toolName="$1"
repoRoot="$2"
overrideScriptsPath="$3"

scriptPath="$(cd "$(dirname "$0")"; pwd -P)"
. "$scriptPath/tool-helper.sh"

exit_if_arg_count_mismatch "$#" "3" "$(usage)"
exit_if_arg_empty "$toolName" "tool-name" "$(usage)"
exit_if_invalid_path "$repoRoot" "repository-root" "$(usage)"
repoRoot="$(cd "$repoRoot"; pwd -P)"
exit_if_invalid_folder "$overrideScriptsFolderPath" "override-scripts-folder-path" "$(usage)" "ignoreEmptyValue"

download_extract()
{
    downloadUrl="$(get_tool_config_value "DownloadUrl" "$toolName" "$repoRoot")" || fail "$repoRoot" "$downloadUrl"
    downloadPackageFilename=$(get_download_file "$toolName" "$repoRoot") || fail "$repoRoot" "$downloadPackageFilename"
    downloadUrl="$downloadUrl$downloadPackageFilename"

    toolFolder="$(get_local_tool_folder "$toolName" "$repoRoot")"
    rm -rf "$toolFolder"
    mkdir -p "$toolFolder"
    downloadPackagePath="$toolFolder/$downloadPackageFilename"
    log_message "$repoRoot" "Attempting to download $toolName from $downloadUrl to $downloadPackagePath."

    # curl has HTTPS CA trust-issues less often than wget, so lets try that first.
    which curl > /dev/null 2> /dev/null

    if [ $? -ne 0 ]; then
        log_message "$repoRoot" "$(wget --tries=10 -v -O "$downloadPackagePath" "$downloadUrl" 2>&1)"
    else
        log_message "$repoRoot" "$(curl --retry 10 -ssl -v -o "$downloadPackagePath" "$downloadUrl" 2>&1)"
    fi

    log_message "$repoRoot" "Attempting to extract $downloadPackagePath to $toolFolder."
    log_message "$repoRoot" "$(tar -xvzf "$downloadPackagePath" -C "$toolFolder" 2>&1)"
}

confirm_toolpath()
{
    toolPath="$(get_local_search_path "$toolName" "$repoRoot")" || fail "$repoRoot" "$toolPath"
    validate_tool_path "$toolPath" "$toolName" "strict" "$repoRoot" "$overrideScriptsPath"

    if [ $? -eq 0 ]; then
        echo "$toolPath"
        exit
    fi

    # Note that acquireScriptPath and acquireScriptArgs are used in ToolNotFoundErrorMessage.
    acquireScriptPath="$scriptPath/acquire-tool.sh"
    acquireScriptArgs="\"$toolName\" \"$repoRoot\" \"$overrideScriptsFolderPath\""
    toolNotFoundMessage=$(tool_not_found_message "$toolName" "$repoRoot") || fail "$repoRoot" "$toolNotFoundMessage"
    log_message "$repoRoot" "$toolNotFoundMessage"
    echo "$toolNotFoundMessage"
    exit 1
}

download_extract

confirm_toolpath
