# Licensed to the .NET Foundation under one or more agreements.
# The .NET Foundation licenses this file to you under the MIT license.
# See the LICENSE file in the project root for more information.

# Helper script used for pointing the current powershell environment 
# to the testhost sdk built by the corefx build script.

[CmdletBinding(PositionalBinding=$false)]
Param(
  [string][Alias('f')]$framework = "netcoreapp",
  [string][Alias('c')]$configuration = "Debug",
  [string][Alias('a')]$arch = "x64",
  [string][Alias('o')]$os = "",
  [switch][Alias('b')]$copyAspNetBits
)

# script needs to be sourced, detect if running standalone
if ($MyInvocation.InvocationName -ne ".")
{
    write-output "Script must be sourced"
    write-output "USAGE: . $($MyInvocation.InvocationName) <args>"
    exit
}

# find corefx root, assuming script lives in the git repo
$SOURCE_DIR="$(split-path -Parent $MyInvocation.MyCommand.Definition)"
$COREFX_ROOT_DIR=$(git -C "$SOURCE_DIR" rev-parse --show-toplevel)

function Find-Os()
{
    if (!$(test-path variable:IsWindows) -or $IsWindows)
    {
        return "Windows_NT"
    } 
    else
    {
        switch -Wildcard ($(uname -s))
        {
            "Linux*" { return "Linux" }
            "Darwin*" { return "MacOS" }
            "*" { return "Unix" }
        }
    }
}

if ($os -eq "")
{
    $os=$(Find-Os)
}

# the corefx testhost does not bundle AspNetCore runtime bits;
# fix up by copying from the bootstrap sdk 
function Copy-Aspnetcore-Bits([string] $testhost_path)
{

    function find-bootstrap-sdk()
    {
        if (test-path -PathType container "$COREFX_ROOT_DIR/.dotnet")
        {
            "$COREFX_ROOT_DIR/.dotnet"
        }
        else
        {
            $dotnet_path = $(which dotnet)

            # follow any symlinks if unix
            if (which readlink)
            {
                $dotnet_path = $(readlink -f $dotnet_path)
            }

            $(dirname $dotnet_path)
        }
    }

    $netfx_bits_folder="Microsoft.NETCore.App"
    $aspnet_bits_folder="Microsoft.AspNetCore.App"

    if (!(test-path -PathType container "$testhost_path/shared/$aspnet_bits_folder"))
    {
        $bootstrap_sdk=$(find-bootstrap-sdk)

        function get-most-recent-version($path)
        {
            (Get-ChildItem -Directory "$path" `
            | Select-Object -ExpandProperty Fullname `
            | Split-Path -Leaf `
            | ForEach-Object{[System.Version]$_} `
            | Sort-Object -Descending `
            | Select-Object -First 1).ToString()
        }
        
        $netfx_runtime_version=$(get-most-recent-version "$testhost_path/shared/$netfx_bits_folder")
        $aspnet_runtime_version=$(get-most-recent-version "$bootstrap_sdk/shared/$aspnet_bits_folder")

        # copy the bits
        mkdir -p "$testhost_path/shared/$aspnet_bits_folder/" > $null
        copy-item -R "$bootstrap_sdk/shared/$aspnet_bits_folder/$aspnet_runtime_version" "$testhost_path/shared/$aspnet_bits_folder/$netfx_runtime_version"
        if (!$?)
        {
            write-host "failed to copy aspnetcore bits"
            return
        }

        $aspNetRuntimeConfig="$testhost_path/shared/$aspnet_bits_folder/$netfx_runtime_version/$aspnet_bits_folder.runtimeconfig.json"
        if (test-path -PathType leaf "$aspNetRuntimeConfig")
        {
            # point aspnetcore runtimeconfig.json to current netfx version
            # would prefer jq here but missing in many distros by default
            $updated_content = $(get-content "$aspNetRuntimeConfig") -replace '"version"\s*:\s*"[^"]*"', "`"version`":`"$netfx_runtime_version`""
            write-output $updated_content | Out-File -FilePath "$aspNetRuntimeConfig" -Encoding utf8
        }

        write-host "Copied Microsoft.AspNetCore.App runtime bits from $bootstrap_sdk"
    }
}

function Set-Sdk-Environment()
{
    $candidate_path=$([IO.Path]::Combine($COREFX_ROOT_DIR, 'artifacts', 'bin', 'testhost', "$FRAMEWORK-$OS-$CONFIGURATION-$ARCH"))

    if (!$(test-path -PathType container $candidate_path))
    {
        write-output "Could not locate testhost sdk path $candidate_path" 
        return
    }
    elseif (!$(test-path -PathType leaf "$candidate_path/dotnet") -and 
            !$(test-path -PathType leaf "$candidate_path/dotnet.exe"))
    {
        write-output "Could not find dotnet executable in testhost sdk path $candidate_path"
        return
    }

    if($copyAspNetBits)
    {
        Copy-Aspnetcore-Bits $candidate_path
    }

    $pathSeparator=if($os -eq "Windows_NT") { ";" } else { ":" }
    
    $env:DOTNET_ROOT=$candidate_path
    $env:DOTNET_CLI_HOME=$candidate_path
    $env:PATH=($candidate_path + $pathSeparator + $env:PATH)
    $env:DOTNET_MULTILEVEL_LOOKUP=0
    $env:DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX=2
}


Set-Sdk-Environment
