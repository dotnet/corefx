$repoRoot = ((get-item $PSScriptRoot).parent.parent.parent.FullName);
$winRID = "win-x64";
$dotnetPath = -join($repoRoot, "\Tools\dotnetcli\dotnet.exe")
$csprojPath = -join($PSScriptRoot, "\", (Get-ChildItem $PSScriptRoot"\*.csproj" | Select-Object -ExpandProperty Name))
$packagesCachePath = -join($repoRoot, "\packages")
$localPackageSourcePath = -join($repoRoot, "\bin\packages\Debug\")
$packageName = "Microsoft.Windows.Compatibility"

function _pathExists($path)
{
	return Test-Path -Path $path
}

if (!(_pathExists $localPackageSourcePath))
{
	$localPackageSourcePath = -join($repoRoot, "\bin\packages\Release\")
	if (!(_pathExists $localPackageSourcePath))
	{
		Write-Error -Message "Local package source must exist.";
		Exit;
	}
}

function _getPackageVersion()
{
	$searchPattern = -join($localPackageSourcePath, $packageName, ".[0-9].[0-9].[0-9]*.nupkg")
	if (!(_pathExists $searchPattern))
	{
		Write-Error -Message (-join("Didn't find package: Microsoft.Windows.Compatibility in source: ", $localPackageSourcePath, " please run build -allConfigurations"))
		Exit;
	}

	if (!((get-item $searchPattern).FullName -match '([0-9].[0-9].[0-9][-a-z0-9]*)'))
	{
		Write-Error -Message "Package name is invalid"
		Exit;
	}

	return $matches[0]
}

function _restoreAndPublish($targetFramework, $outputType, $rid, $runtimeFramework)
{
	$packageVersion = _getPackageVersion
    & $dotnetPath restore --packages $packagesCachePath /p:RestoreSources="https://api.nuget.org/v3/index.json;$localPackageSourcePath" /p:TargetFramework=$targetFramework /p:CompatibilityPackageVersion=$packageVersion $csprojPath
    & $dotnetPath publish -r $rid /p:RestoreSources="https://api.nuget.org/v3/index.json;$localPackageSourcePath" /p:TargetFramework=$targetFramework /p:CompatibilityPackageVersion=$packageVersion /p:RuntimeFrameworkVersion=$runtimeFramework $csprojPath

    $outputPath = -join($PSScriptRoot, "\bin\Debug\", $targetFramework, "\", $rid, "\publish\refs\")

    if (!(_pathExists $outputPath))
    {
    	Write-Error -Message (-join("There was an error while publishing for framework: ", $targetFramework))
		Exit;
    }

    echo (-join("Published succedded for: ", $targetFramework))
}

_restoreAndPublish "netcoreapp2.0" "Exe" $winRID "2.0.0"
_restoreAndPublish "netstandard2.0" "Library" $winRID "2.0.0"