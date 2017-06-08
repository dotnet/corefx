<#
.SYNOPSIS
    Searches for the tool in the environment path, and paths specified for the tool in the .toolversions file. 
    If search is successful, then returns the path to the tool.
.PARAMETER ToolName
    Name of the tool to search.
.PARAMETER RepositoryRoot
    Path to repository root.
.PARAMETER OverrideScriptsFolderPath
    If a path is specified, then scripts from the specified folder will be invoked. 
    Otherwise, the default scripts located within the repository will be invoked.
.PARAMETER ExtraArgs
    Additional parameters. For example, specifying StrictToolVersionMatch switch will ensure that the version of the tool searched is the declared version. 
    Otherwise, search will attempt to find a version of the tool, which may not be the declared version.
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
    [parameter(ValueFromRemainingArguments=$true)]
    [string]$ExtraArgs
)

. $PSScriptRoot\tool-helper.ps1

function Validate-SearchResult
{
    return Validate-Toolpath -ToolPath "$toolPath" -ToolName "$ToolName" -StrictToolVersionMatch $StrictToolVersionMatch -RepositoryRoot "$RepositoryRoot" -OverrideScriptsFolderPath "$OverrideScriptsFolderPath"
}

function Get-EnvironmentTool
{
    Write-LogMessage "Searching for $ToolName in environment path." "$RepositoryRoot"
    $toolPath = (Get-Command "$ToolName" -ErrorAction SilentlyContinue).Path

    if (Validate-SearchResult)
    {
        return "$toolPath"
    }
}

function Get-InstallLocationsTool
{
    Write-LogMessage "Searching for $ToolName in the install locations specified in the .toolversions file." "$RepositoryRoot"
    $searchPaths = Get-ToolConfigValue "SearchPathsWindows" "$ToolName" "$RepositoryRoot" -IsMultiLine
    $searchPaths = NormalizeSearchPath $searchPaths

    foreach ($toolPath in $searchPaths)
    {
        Write-LogMessage "Searching for $ToolName in $toolPath." "$RepositoryRoot"

        if (Validate-SearchResult)
        {
            return "$toolPath"
        }
    }
}

function Get-CacheTool
{
    Write-LogMessage "Searching for $ToolName in the local tools cache." "$RepositoryRoot"
    $toolPath = Get-LocalSearchPath "$ToolName" "$RepositoryRoot"

    if (Validate-SearchResult)
    {
        return "$toolPath"
    }
}

$searchResult = Get-EnvironmentTool

if ([string]::IsNullOrWhiteSpace($searchResult))
{
    $searchResult = Get-InstallLocationsTool

    if ([string]::IsNullOrWhiteSpace($searchResult))
    {
        $searchResult = Get-CacheTool
    }
}

return "$searchResult"
