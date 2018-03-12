#
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

# This script initializes tools if necessary, then calls MSBuild using the
# dotnet CLI. This allows Maestro to run auto-update targets without
# encountering errors when contacting GitHub: .NET Core CLI supports TLS 1.2
# by default, but full framework doesn't.

$toolsLocalPath = Join-Path $PSScriptRoot "Tools"

$initTools = Join-Path $PSScriptRoot "init-tools.cmd"
& $initTools

# Execute MSBuild using the dotnet.exe host
$dotNetExe = Join-Path $toolsLocalPath "dotnetcli\dotnet.exe"
$msbuildExe = Join-Path $toolsLocalPath "msbuild.exe"
$testsBuildProj = Join-Path $PSScriptRoot "build.proj"
& $dotNetExe $msbuildExe $testsBuildProj /t:UpdateDependenciesAndSubmitPullRequest $args
exit $LastExitCode