#
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

# Updates the ValidDependencyVersions.txt file with the latest CoreFX build number
function UpdateValidDependencyVersionsFile
{
    # TODO: change to rc3 and get this from a variable
    $CoreFxLatestVersion = Invoke-WebRequest https://raw.githubusercontent.com/eerhardt/versions/master/corefx/release/1.0.0-rc2/Latest.txt -UseBasicParsing
    $CoreFxLatestVersion = $CoreFxLatestVersion.ToString().Trim()

    $ValidDependencyVersionsPath = "$PSScriptRoot\ValidDependencyVersions.txt"
    $ValidDependencyVersionsText = [IO.File]::ReadAllText($ValidDependencyVersionsPath)

    $CoreFxRegex = new-object Text.RegularExpressions.Regex 'CoreFX_ExpectedPrerelease=.*\d+'

    $ValidDependencyVersionsText = $CoreFxRegex.Replace($ValidDependencyVersionsText, "CoreFX_ExpectedPrerelease=$CoreFxLatestVersion")

    [IO.File]::WriteAllText($ValidDependencyVersionsPath, $ValidDependencyVersionsText, [Text.Encoding]::UTF8)
}

# Updates all the project.json files with out of date version numbers
function RunUpdatePackageDependencyVersions
{
    cmd /c $PSScriptRoot\build.cmd /t:UpdateInvalidPackageVersions | Out-Null
}

# Creates a Pull Request for the updated version numbers
function CreatePullRequest
{
    git add .
    $UserName = $env:GITHUB_USER
    $Email = $env:GITHUB_EMAIL

    $CommitMessage = 'Updating dependencies from latest build numbers'
    git commit -m "$CommitMessage" --author "$UserName <$Email>"

    $RemoteUrl = "github.com/$UserName/corefx.git"
    $RemoteBranchName = "UpdateDependencies$([DateTime]::UtcNow.ToString('yyyyMMddhhmmss'))"
    $RefSpec = "HEAD:refs/heads/$RemoteBranchName"

    Write-Host "git push https://$RemoteUrl $RefSpec"
    # pipe this to null so the password secret isn't in the logs
    git push "https://$($UserName):$env:GITHUB_PASSWORD@$RemoteUrl" $RefSpec 2>&1 | Out-Null

    $CreatePRBody = @"
    {
        "title": "$CommitMessage", 
        "head": "$($UserName):$RemoteBranchName", 
        "base": "master" 
    }
"@

    $CreatePRHeaders = @{'Accept'='application/vnd.github.v3+json'; 'Authorization'="token $env:GITHUB_PASSWORD"}

    Invoke-WebRequest https://api.github.com/repos/dotnet/corefx/pulls -Method Post -Body $CreatePRBody -Headers $CreatePRHeaders
}

UpdateValidDependencyVersionsFile
RunUpdatePackageDependencyVersions
CreatePullRequest