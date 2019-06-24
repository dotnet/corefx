function _getPackageVersion($packageName)
{
	$searchPattern = -join($localPackageSourcePath, $packageName, ".[0-9].[0-9].[0-9]*.nupkg")
	if (!(Test-Path $searchPattern))
	{
		Write-Error -Message (-join("Didn't find package: ", $packageName, " in source: ", $localPackageSourcePath, " please run build -allConfigurations"))
		Exit;
	}

	if (!([string]((get-item $searchPattern).FullName) -match '([0-9].[0-9].[0-9][-a-z]*.[0-9]*.[0-9]*)'))
	{
		Write-Error -Message "Package name is invalid"
		Exit;
	}

	if (!$matches)
	{
		Write-Error -Message (-join("Couldn't get package version for: ", $packageName))
		Exit;
	}
	return $matches[1]
}

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1

$repoRoot = ((get-item $PSScriptRoot).parent.parent.parent.FullName)
$dotnetPath = -join($repoRoot, "\.dotnet\dotnet.exe")
$csprojPath = -join($PSScriptRoot, "\", (Get-ChildItem $PSScriptRoot"\*.csproj" | Select-Object -ExpandProperty Name))
$localPackageSourcePath = -join($repoRoot, "\artifacts\packages\Debug\")
$restoreOutputPath = -join($repoRoot, "\artifacts\ApiCatalogLayout\")

if (!(Test-Path $localPackageSourcePath))
{
	$localPackageSourcePath = -join($repoRoot, "\artifacts\packages\Release\")
	if (!(Test-Path $localPackageSourcePath))
	{
		Write-Error -Message (-join('Local package source must exist ', $localPackageSourcePath))
		Exit;
	}
}

$restoreSources = -join("https://dotnetfeed.blob.core.windows.net/dotnet-core/index.json;https://dotnetfeed.blob.core.windows.net/aspnet-aspnetcore/index.json;", $localPackageSourcePath)

$compatPackageVersion = _getPackageVersion "Microsoft.Windows.Compatibility"
$privateNetcoreappPackageVersion = _getPackageVersion "Microsoft.Private.CoreFx.NETCoreApp"
$privateUAPPackageVersion = _getPackageVersion "Microsoft.Private.CoreFx.UAP"
$systemWindowsExtensionsVersion = _getPackageVersion "System.Windows.Extensions"

Write-Output "Generating APICatalog Layout"
& $dotnetPath msbuild /t:GetReferences /p:RestoreSources="$restoreSources" /p:CompatibilityPackageVersion=$compatPackageVersion /p:PrivateCorefxNetCoreAppPackageVersion=$privateNetcoreappPackageVersion /p:PrivateCorefxUAPPackageVersion=$privateUAPPackageVersion /p:SystemWindowsExtensionsVersion=$systemWindowsExtensionsVersion /p:RestoreOutputPath=$restoreOutputPath $csprojPath

if (!(Test-Path $restoreOutputPath))
{
	Write-Error -Message "There was an error while generating the APICatalog layout"
	Exit;
}

Write-Output "Successfully generated APICatalog Layout"

$refPath = -join($repoRoot, "\artifacts\bin\ref\", $refDirName)
