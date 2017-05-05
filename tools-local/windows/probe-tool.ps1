<#
.SYNOPSIS
    Invokes an extension that calls the appropriate search and/or acquire scripts. 
    ToolName, OverrideScriptsFolderPath and StrictToolVersionMatch parameters are passed on to the extension.
.PARAMETER ToolName
    Name of the tool to search and/or download.
.PARAMETER RepositoryRoot
    Path to repository root.
.PARAMETER OverrideScriptsFolderPath
    If a path is specified, then search and acquire scripts from the specified folder will be invoked. 
    Otherwise, search will use the default search and acquire scripts located within the repository.
.PARAMETER StrictToolVersionMatch
    If specified, then search will ensure that the version of the tool searched is the declared version. 
    Otherwise, search will attempt to find a version of the tool, which may not be the declared version.
.EXAMPLE
    .\probe-tool.ps1 cmake "C:\Users\dotnet\Source\Repos\corefx" ""
    Probes for CMake, not necessarily the declared version, using the default search and acquire scripts located within the repository.
.EXAMPLE
    .\probe-tool.ps1 cmake "C:\Users\dotnet\Source\Repos\corefx" cmake "" -StrictToolVersionMatch
    Probes for the declared version of CMake using the default search and acquire scripts located within the repository.
.EXAMPLE
    .\probe-tool.ps1 cmake "C:\Users\dotnet\Source\Repos\corefx" "D:\dotnet\MyCustomScripts" -StrictToolVersionMatch
    Probes for the declared version of CMake using the search and acquire scripts located in the override folder at "D:\dotnet\MyCustomScripts".
    If a search or acquire script is not available in the override folder, then the default script located within the repository will be invoked.
#>

[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [parameter(Mandatory=$true, Position=0)]
    [string]$ToolName,
    [ValidateNotNullOrEmpty()]
    [parameter(Mandatory=$true, Position=1)]
    [string]$RepositoryRoot,
    [parameter(Position=2)]
    [string]$OverrideScriptsFolderPath,
    [switch]$StrictToolVersionMatch
)

$oldEAP = $ErrorActionPreference

try
{
    $ErrorActionPreference = 'Stop'
    $RepositoryRoot = [System.IO.Path]::GetFullPath($RepositoryRoot)
    . $PSScriptRoot\tool-helper.ps1

    if (-not [string]::IsNullOrWhiteSpace($OverrideScriptsFolderPath) -and -not (Test-Path $OverrideScriptsFolderPath -PathType Container))
    {
        Write-Error "Path specified as OverrideScriptsFolderPath does not exist. Path: $OverrideScriptsFolderPath"
    }

    $toolPath = Invoke-ExtensionScript "search-tool.ps1" "$ToolName" "$RepositoryRoot" "$OverrideScriptsFolderPath" -StrictToolVersionMatch $StrictToolVersionMatch

    if ([string]::IsNullOrWhiteSpace($toolPath) -or -not (Test-Path $toolPath -PathType Leaf))
    {
        $toolPath = Invoke-ExtensionScript "acquire-tool.ps1" "$ToolName" "$RepositoryRoot" "$OverrideScriptsFolderPath"
    }

    return "$toolPath"
}
finally
{
    $ErrorActionPreference = $oldEAP
}
