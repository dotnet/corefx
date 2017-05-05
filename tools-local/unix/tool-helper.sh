#!/usr/bin/env bash

# Provides helper functions.

exit_if_arg_count_mismatch()
{
    actualArgsCount="$1"
    expectedArgsCount="$2"
    usage="$3"

    if [ $actualArgsCount -ne $expectedArgsCount ]; then
        echo "Invalid number of arguments specified. Actual: $actualArgsCount Expected: $expectedArgsCount"
        echo "$usage"
        exit 1
    fi
}

exit_if_invalid_path()
{
    pathValue="$1"
    pathName="$2"
    usage="$3"

    exit_if_arg_empty "$pathValue" "$pathName" "$usage"

    if [ ! -f "$pathValue" ]; then
        exit_if_invalid_folder "$pathValue" "$pathName" "$usage"
    fi
}

exit_if_invalid_folder()
{
    pathValue="$1"
    pathName="$2"
    usage="$3"
    ignoreEmptyValue="$4"

    if [ -z "$pathValue" ] && [ "$ignoreEmptyValue" == "ignoreEmptyValue" ]; then
        return
    fi

    if [ ! -d "$pathValue" ]; then
        echo "Path specified as $pathName does not exist. Path: $pathValue"
        echo "$usage"
        exit 1
    fi
}

exit_if_arg_empty()
{
    argValue="$1"
    argName="$2"
    usage="$3"

    if [ -z "$argValue" ]; then
        echo "Argument passed as $argName is empty. Please provide a non-empty string."
        echo "$usage"
        exit 1
    fi
}

validate_tool_name_repo_root_args()
{
    toolName="$1"
    repoRoot="$2"

    exit_if_arg_empty "$toolName" "tool-name"
    exit_if_invalid_path "$repoRoot" "repository-root"
}

get_default_scripts_folder()
{
    repoRoot="$1"
    exit_if_invalid_path "$repoRoot" "repository-root"
    echo "$repoRoot/tools-local/unix"
}

get_os_name()
{
    osName="$(uname -s)"

    if [ $? -ne 0 ] || [ -z "$osName" ]; then
        echo "Unable to determine the name of the operating system."
        exit 1
    fi

    if echo "$osName" | grep -iqF "Darwin"; then
        osName="OSX"
    else
        osName="Linux"
    fi

    echo "$osName"
}

eval_tool()
{
    toolName="$1"
    repoRoot="$2"

    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    . "$repoRoot/.toolversions"

    # Evaluate toolName. This assigns the metadata of toolName to tools.
    eval "tools=\$$toolName"

    # Evaluate tools. Each argument here is tool specific data such as DeclaredVersion of toolName.
    eval "$tools"
}

get_tool_config_value()
{
    configName="$1"
    toolName="$2"
    repoRoot="$3"

    exit_if_arg_empty "configuration-name" "$configName"
    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    configValue="$(eval_tool "$toolName" "$repoRoot"; eval echo "\$$configName")"

    if [ -z "$configValue" ]; then
        echo "Unable to read the value corresponding to $configName from the .toolversions file."
        exit 1
    fi

    echo "$configValue"
}

get_download_file()
{
    toolName="$1"
    repoRoot="$2"

    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    osName="$(get_os_name)"
    get_tool_config_value "DownloadFile$osName" "$toolName" "$repoRoot"
}

get_local_tool_folder()
{
    toolName="$1"
    repoRoot="$2"

    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    toolFolder="$(get_tool_config_value "LocalToolFolder" "$toolName" "$repoRoot")"

    if [ $? -ne 0 ]; then
        toolFolder="Tools/downloads/$toolName"
    fi

    case "$toolFolder" in
        /*)
            echo "$toolFolder"
            ;;
        *)
            # Assumed that the path specified in .toolversion is relative to the repository root.
            echo "$repoRoot/$toolFolder"
            ;;
    esac
}

get_local_search_path()
{
    toolName="$1"
    repoRoot="$2"

    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    toolFolder="$(get_local_tool_folder "$toolName" "$repoRoot")"
    osName="$(get_os_name)"
    searchPath="$(get_tool_config_value "LocalSearchPath${osName}" "$toolName" "$repoRoot")"
    echo "$toolFolder/$searchPath"
}

tool_not_found_message()
{
    toolName="$1"
    repoRoot="$2"

    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    # Eval in a subshell to avoid conflict with existing variables.
    (
        eval_tool "$toolName" "$repoRoot"
        [ ! -z "$ToolNotFoundMessage" ] || fail "Unable to locate $toolName." "$repoRoot"
        eval echo "$ToolNotFoundMessage"
    )
}

log_message()
{
    repoRoot="$1"
    exit_if_invalid_path "$repoRoot" "repository-root"

    probeLog="$repoRoot/probe-tool.log"
    shift
    echo "$*" >> "$probeLog"
}

fail()
{
    log_message "$repoRoot" "$@"
    shift
    echo "$@"
    exit 1
}

validate_tool_path()
{
    toolPath="$1"
    toolName="$2"
    strictToolVersionMatch="$3"
    repoRoot="$4"
    overrideScriptsPath="$5"

    exit_if_invalid_path "$toolPath" "toolPath"
    validate_tool_name_repo_root_args "$toolName" "$repoRoot"

    toolVersion="$(invoke_extension "get-version.sh" "$toolName" "$repoRoot" "$overrideScriptsPath" "$toolPath")" || fail "$repoRoot" "$toolVersion"
    log_message "$repoRoot" "$toolName version $toolVersion is at $toolPath."

    if [ "$strictToolVersionMatch" == "strict" ]; then
        declaredVersion="$(get_tool_config_value "DeclaredVersion" "$toolName" "$repoRoot")" || fail "$repoRoot" "$declaredVersion"

        if [ "$toolVersion" == "$declaredVersion" ]; then
            log_message "$repoRoot" "Version matches the declared version $declaredVersion."
        else
            log_message "$repoRoot" "Version does not match the declared version $declaredVersion."
            return 1
        fi
    fi
}

invoke_extension()
{
    extensionScriptName="$1"
    toolName="$2"
    repoRoot="$3"
    overrideScriptsFolderPath="$4"

    [ $# -ge 4 ] || fail "$repoRoot" "Invalid number of arguments specified. Actual: $# Expected: 4"
    exit_if_arg_empty "$extensionScriptName" "script-name" "$(usage)"
    exit_if_arg_empty "$toolName" "tool-name" "$(usage)"
    exit_if_invalid_path "$repoRoot" "repository-root" "$(usage)"

    if [ ! -z "$overrideScriptsFolderPath" ]; then
        [ -d "$overrideScriptsFolderPath" ] || 
        fail "$repoRoot" "Path specified as override-scripts-folder-path does not exist. Path: $overrideScriptsFolderPath" "$(usage)"
    fi

    defaultScriptsFolderPath="$(get_default_scripts_folder $repoRoot)"

    # Searches for an override of the extension script. If an override does not exist, then gets the path to base implementation of the script.
    for extensionsFolder in "$overrideScriptsFolderPath" "$defaultScriptsFolderPath"; do
        if [ -d "$extensionsFolder" ]; then
            invokeScriptPath="$extensionsFolder/$toolName/$extensionScriptName"

            if [ -f "$invokeScriptPath" ]; then
                break
            fi

            invokeScriptPath="$extensionsFolder/$extensionScriptName"

            if [ -f "$invokeScriptPath" ]; then
                break
            fi
        fi
    done

    log_message "$repoRoot" "Invoking $extensionScriptName located in $(dirname $invokeScriptPath) with the following arguments $@."

    # Note that the first argument is the name of the extension script. Hence shift, and pass rest of the arguments to the invocation.
    shift
    "$invokeScriptPath" "$@"
}
