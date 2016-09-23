﻿#
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

# This script updates the dotnet/versions repository based on a set of packages. It directly
# commits the changes using GitHub APIs.

param(
    [Parameter(Mandatory=$true)][string]$gitHubUser,
    [Parameter(Mandatory=$true)][string]$gitHubEmail,
    [Parameter(Mandatory=$true)][string]$gitHubAuthToken,
    [Parameter(Mandatory=$true)][string]$versionsRepoOwner,
    [Parameter(Mandatory=$true)][string]$versionsRepo,
    [Parameter(Mandatory=$true)][string]$versionsRepoPath,
    # A pattern matching all packages in the set that the versions repository should be set to.
    [Parameter(Mandatory=$true)][string]$nupkgPath)

. "$PSScriptRoot\build-managed.cmd" -- /t:UpdatePublishedVersions `
    /p:GitHubUser="$gitHubUser" `
    /p:GitHubEmail="$gitHubEmail" `
    /p:GitHubAuthToken="$gitHubAuthToken" `
    /p:VersionsRepoOwner="$versionsRepoOwner" `
    /p:VersionsRepo="$versionsRepo" `
    /p:VersionsRepoPath="$versionsRepoPath" `
    /p:ShippedNuGetPackageGlobPath="$nupkgPath"
