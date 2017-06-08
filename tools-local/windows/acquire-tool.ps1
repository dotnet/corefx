<#
.SYNOPSIS
    Downloads the declared version of the specified tool from the corresponding URL specified in the .toolversions file. 
    If download succeeds, then returns the path to the executable.
.PARAMETER ToolName
    Name of the tool to download.
.PARAMETER RepositoryRoot
    Path to repository root.
.PARAMETER OverrideScriptsFolderPath
    If a path is specified, then scripts from the specified folder will be invoked. 
    Otherwise, the default scripts located within the repository will be invoked.
.PARAMETER ExtraArgs
    Additional parameters passed to this script. These are ignored.
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

$oldEAP = $ErrorActionPreference

try
{
    $ErrorActionPreference = 'Stop'
    . $PSScriptRoot\tool-helper.ps1

    function Start-DownloadExtract
    {
        # Get the download URL
        $downloadUrl = Get-ToolConfigValue "DownloadUrl" "$ToolName" "$RepositoryRoot"
        $downloadPackageFilename = Get-DownloadFile "$ToolName" "$RepositoryRoot"
        $downloadUrl = "$downloadUrl$downloadPackageFilename"

        $toolFolder = Get-LocalToolFolder "$ToolName" "$RepositoryRoot"
        Remove-Item -Path "$toolFolder" -Recurse -Force -ErrorAction SilentlyContinue
        New-Item -Path "$toolFolder" -ItemType Directory | Out-Null
        $downloadPackagePath = Join-Path "$toolFolder" "$downloadPackageFilename"

        Write-LogMessage "Attempting to download $ToolName from $downloadUrl to $downloadPackagePath." "$RepositoryRoot"
        $downloadLog = Invoke-WebRequest -Uri $downloadUrl -OutFile $downloadPackagePath -DisableKeepAlive -UseBasicParsing -PassThru
        Write-LogMessage "Download Status Code: $($downloadLog.StatusCode)" "$RepositoryRoot"

        Write-LogMessage "Attempting to extract $downloadPackagePath to $toolFolder." "$RepositoryRoot"
        Expand-Archive -Path $downloadPackagePath -DestinationPath $toolFolder -Force | Out-Null
        Write-LogMessage "Extracted successfully to $toolFolder." "$RepositoryRoot"
    }

    function Confirm-Toolpath
    {
        $toolPath = Get-LocalSearchPath "$ToolName" "$RepositoryRoot"

        if (-not (Validate-Toolpath -ToolPath "$toolPath" -ToolName "$ToolName" -StrictToolVersionMatch $true -RepositoryRoot "$RepositoryRoot" -OverrideScriptsFolderPath "$OverrideScriptsFolderPath"))
        {
            $toolNotFoundMessage = Get-ToolNotFoundMessage "$ToolName" "$RepositoryRoot" -GetToolScriptPath "$([System.IO.Path]::GetFullPath("$PSScriptRoot\acquire-tool.ps1"))" -GetToolScriptArgs "`"$ToolName`" `"$RepositoryRoot`""
            Write-LogMessage "$toolNotFoundMessage" "$RepositoryRoot"
            Write-Error "$toolNotFoundMessage"
        }

        return "$toolPath"
    }

    Start-DownloadExtract

    return Confirm-Toolpath
}
finally
{
    $ErrorActionPreference = $oldEAP
}
