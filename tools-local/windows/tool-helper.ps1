# Provides helper functions.

function Get-DefaultScriptsFolder
{
    [CmdletBinding()]
    param(
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=0)]
        [string]$RepositoryRoot
    )

    return Join-Path "$RepositoryRoot" "tools-local\windows"
}

function Get-OperatingSystemArchitecture
{
    if ([System.Environment]::Is64BitOperatingSystem)
    {
        return "64"
    }

    return "32"
}

function Get-ToolVersionsConfig
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot
    )

    $toolVersionsFilePath = Join-Path "$RepositoryRoot" ".toolversions"
    $toolVersions = Get-Content -Path $toolVersionsFilePath -Raw
    $regexPattern = "$ToolName=.*([`\n]|.*)+?`\"""
    $toolConfig = [regex]::Match($toolVersions, $regexPattern).Value

    return "$toolConfig"
}

function Get-ToolConfigValue
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ConfigName,
        [parameter(Mandatory=$true, Position=1)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=2)]
        [string]$RepositoryRoot,
        [switch]$AllowNullOrEmptyConfigValue,
        [switch]$IsMultiLine
    )

    $toolConfig = Get-ToolVersionsConfig "$ToolName" "$RepositoryRoot"
    $regexPattern = "(?<=$ConfigName=')[^']*"
    $match = [regex]::Match($toolConfig, $regexPattern)

    if (-not $AllowNullOrEmptyConfigValue -and -not $match.Success)
    {
        Write-Error "Unable to read the value corresponding to $ConfigName from the .toolversions file."
    }

    $configValue = $match.Value

    if (-not $IsMultiLine)
    {
        return "$configValue"
    }

    $configValue = $configValue.Split([Environment]::NewLine, [System.StringSplitOptions]::RemoveEmptyEntries)
    $multilineValues = @()
    $configValue | % { $multilineValues += $_.Trim() }
    return $multilineValues
}

function Get-DownloadFile
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot
    )

    $configName = "DownloadFileWindows"
    $configName += Get-OperatingSystemArchitecture
    $downloadFile = Get-ToolConfigValue "$configName" "$ToolName" "$RepositoryRoot"
    return "$downloadFile"
}

function Get-LocalToolFolder
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot
    )

    $toolFolder = Get-ToolConfigValue "LocalToolFolderWindows" "$ToolName" "$RepositoryRoot" -AllowNullOrEmptyConfigValue -ErrorAction SilentlyContinue

    if ([string]::IsNullOrWhiteSpace($toolFolder) -or -not (Test-Path "$toolFolder" -PathType Container))
    {
        $toolFolder = Join-Path "$RepositoryRoot" "Tools\downloads\$ToolName"
    }

    if (-not [System.IO.Path]::IsPathRooted($toolFolder))
    {
        $toolFolder = Join-Path "$RepositoryRoot" "$toolFolder"
    }

    return "$toolFolder"
}

function NormalizeSearchPath
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string[]]$SearchPaths
    )

    $fixedSearchPaths = @()

    foreach ($path in $searchPaths)
    {
        $path = $path.Replace("%programfiles(x86)%", "${env:ProgramFiles(x86)}")
        $path = $path.Replace("%programfiles%", "${env:ProgramFiles}")
        $path = $path.Replace("\\","\")
        $fixedSearchPaths += $path
    }

    return $fixedSearchPaths
}

function Get-LocalSearchPath
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot
    )

    $configName = "LocalSearchPathWindows"
    $configName += Get-OperatingSystemArchitecture
    $searchPath = Get-ToolConfigValue "$configName" "$ToolName" "$RepositoryRoot"

    $toolFolder = Get-LocalToolFolder "$ToolName" "$RepositoryRoot"
    $searchPath = Join-Path "$toolFolder" "$searchPath"
    $searchPath = NormalizeSearchPath $searchPath
    return "$searchPath"
}

function Get-ToolNotFoundMessage
{
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true, Position=0)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot,
        [string]$GetToolScriptPath,
        [string]$GetToolScriptArgs
    )

    $ToolNotFoundMessage = Get-ToolConfigValue "ToolNotFoundMessage" "$ToolName" "$RepositoryRoot"

    # Expand $DeclaredVersion and $DownloadUrl in $ToolNotFoundMessage.
    $DeclaredVersion = Get-ToolConfigValue "DeclaredVersion" "$ToolName" "$RepositoryRoot"
    $DownloadUrl = Get-ToolConfigValue "DownloadUrl" "$ToolName" "$RepositoryRoot"
    $ToolNotFoundMessage = $ToolNotFoundMessage.Replace("\`$", "`$")
    $acquireScriptPath = $GetToolScriptPath
    $acquireScriptArgs = $GetToolScriptArgs
    $ToolNotFoundMessage = $ExecutionContext.InvokeCommand.ExpandString($ToolNotFoundMessage)

    return "$ToolNotFoundMessage"
}

function Write-LogMessage
{
    [CmdletBinding()]
    param(
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=0)]
        [string]$Message,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$RepositoryRoot
    )

    $oldEAP = $ErrorActionPreference

    try
    {
        $ErrorActionPreference = 'SilentlyContinue'
        $probeLog = Join-Path "$RepositoryRoot" "probe-tool.log"
        $Message | Out-File -FilePath "$probeLog" -Append -Force
    }
    catch
    {
        # Do nothing.
    }
    finally
    {
        $ErrorActionPreference = $oldEAP
    }
}

function Invoke-ExtensionScript
{
    [CmdletBinding()]
    param(
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=0)]
        [string]$ScriptName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=1)]
        [string]$ToolName,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=2)]
        [string]$RepositoryRoot,
        [parameter(Position=3)]
        [string]$OverrideScriptsFolderPath,
        [parameter(ValueFromRemainingArguments=$true)]
        [string]$ExtraArgs
    )

    if (-not [string]::IsNullOrWhiteSpace($OverrideScriptsFolderPath) -and -not (Test-Path $OverrideScriptsFolderPath -PathType Container))
    {
        Write-Error "Path specified as override-scripts-folder-path does not exist. Path: $OverrideScriptsFolderPath"
    }

    $defaultScriptsFolderPath = Get-DefaultScriptsFolder "$RepositoryRoot"
    $extensionFolders = $OverrideScriptsFolderPath,$defaultScriptsFolderPath

    foreach ($extFolder in $extensionFolders)
    {
        if (-not [string]::IsNullOrWhiteSpace($extFolder) -and (Test-Path $extFolder -PathType Container -ErrorAction SilentlyContinue))
        {
            $invokeScriptPath = Join-Path "$extFolder\$ToolName" "$ScriptName"

            if (Test-Path $invokeScriptPath -PathType Leaf)
            {
                break
            }

            $invokeScriptPath = Join-Path "$extFolder" "$ScriptName"

            if (Test-Path $invokeScriptPath -PathType Leaf)
            {
                break
            }
        }
    }

    # Note that the first argument is the name of the extension script. Hence remove ScriptName, and pass rest of the arguments to the invocation.
    $PSBoundParameters.Remove("ScriptName") | Out-Null
    $remainingArgs = @()
    $PSBoundParameters.Values | % { $remainingArgs += "`"$_`"" }

    Write-LogMessage "Invoking $invokeScriptPath with the following arguments $remainingArgs." "$RepositoryRoot"
    $output = Invoke-Expression "$invokeScriptPath $remainingArgs"
    return "$output"
}

function Validate-Toolpath
{
    [CmdletBinding()]
    param(
        [parameter(Position=0)]
        [string]$ToolPath,
        [parameter(Mandatory=$true, Position=1)]
        [string]$ToolName,
        [parameter(Mandatory=$true, Position=2)]
        [bool]$StrictToolVersionMatch,
        [ValidateNotNullOrEmpty()]
        [parameter(Mandatory=$true, Position=3)]
        [string]$RepositoryRoot,
        [parameter(Position=4)]
        [string]$OverrideScriptsFolderPath
    )

    if ([string]::IsNullOrWhiteSpace($ToolPath) -or -not (Test-Path $ToolPath -PathType Leaf))
    {
        Write-LogMessage "ToolPath does not exist. ToolPath: $ToolPath." "$RepositoryRoot"
        return $false
    }

    $toolVersion = Invoke-ExtensionScript "get-version.ps1" "$ToolName" "$RepositoryRoot" "$OverrideScriptsFolderPath" "$ToolPath"
    Write-LogMessage "$ToolName version $toolVersion is at $ToolPath." "$RepositoryRoot"

    if ($StrictToolVersionMatch)
    {
        $declaredVersion = Get-ToolConfigValue "DeclaredVersion" "$ToolName" "$RepositoryRoot"

        if ("$toolVersion" -eq "$declaredVersion")
        {
            Write-LogMessage "Version matches the declared version $declaredVersion." "$RepositoryRoot"
            return $true
        }
        else
        {
            Write-LogMessage "Version does not match the declared version $declaredVersion." "$RepositoryRoot"
            return $false
        }
    }

    return $true
}
