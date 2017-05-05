<#
.SYNOPSIS
    Gets the version number of CMake executable at the specified path.
.PARAMETER ToolName
    This should be equal to cmake.
.PARAMETER RepositoryRoot
    This argument is ignored.
.PARAMETER OverrideScriptsFolderPath
    This argument is ignored.
.PARAMETER ToolPath
    Path to CMake executable.
#>

[CmdletBinding()]
param(
    [parameter(Mandatory=$true, Position=0)]
    [string]$ToolName,
    [parameter(Mandatory=$true, Position=1)]
    [string]$RepositoryRoot,
    [parameter(Position=2)]
    [string]$OverrideScriptsFolderPath,
    [ValidateNotNullOrEmpty()]
    [parameter(Mandatory=$true, Position=3)]
    [string]$ToolPath
)

if ($ToolName -ne "cmake")
{
    Write-Error "ToolName argument should be equal to cmake."
}

if (-not (Test-Path -Path $ToolPath -PathType Leaf))
{
    Write-Error "Argument specified as ToolPath does not exist. Path: $ToolPath"
}

# Extract version number. For example, 3.6.0 in text below:
# cmake version 3.6.0
$toolVersion = & $ToolPath -version
$regexPattern = "(?<=cmake version )[0-9][.]([0-9]|[.])+"
$match = [regex]::Match($toolVersion, $regexPattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)

if (-not $match.Success)
{
    Write-Error "Unable to determine the version of CMake at $ToolPath."
}

return "$($match.Value)"
