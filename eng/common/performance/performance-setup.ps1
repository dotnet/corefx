Param(
    [string] $SourceDirectory=$env:BUILD_SOURCESDIRECTORY,
    [string] $CoreRootDirectory,
    [string] $Architecture="x64",
    [string] $Framework="netcoreapp3.0",
    [string] $CompilationMode="Tiered",
    [string] $Repository=$env:BUILD_REPOSITORY_NAME,
    [string] $Branch=$env:BUILD_SOURCEBRANCH,
    [string] $CommitSha=$env:BUILD_SOURCEVERSION,
    [string] $BuildNumber=$env:BUILD_BUILDNUMBER,
    [string] $RunCategories="coreclr corefx",
    [string] $Csproj="src\benchmarks\micro\MicroBenchmarks.csproj",
    [string] $Kind="micro",
    [switch] $Internal,
    [string] $Configurations="CompilationMode=$CompilationMode"
)

$RunFromPerformanceRepo = ($Repository -eq "dotnet/performance")
$UseCoreRun = ($CoreRootDirectory -ne [string]::Empty)

$PayloadDirectory = (Join-Path $SourceDirectory "Payload")
$PerformanceDirectory = (Join-Path $PayloadDirectory "performance")
$WorkItemDirectory = (Join-Path $SourceDirectory "workitem")
$Creator = ""

if ($Internal) {
    $Queue = "Windows.10.Amd64.ClientRS1.Perf"
    $PerfLabArguments = "--upload-to-perflab-container"
    $ExtraBenchmarkDotNetArguments = ""
    $Creator = ""
    $HelixSourcePrefix = "official"
}
else {
    if ($Framework.StartsWith("netcoreapp")) {
        $Queue = "Windows.10.Amd64.ClientRS4.Open"
    }
    else {
        $Queue = "Windows.10.Amd64.ClientRS4.DevEx.15.8.Open"
    }
    $ExtraBenchmarkDotNetArguments = "--iterationCount 1 --warmupCount 0 --invocationCount 1 --unrollFactor 1 --strategy ColdStart --stopOnFirstError true"
    $Creator = $env:BUILD_DEFINITIONNAME
    $PerfLabArguments = ""
    $HelixSourcePrefix = "pr"
}

$CommonSetupArguments="--frameworks $Framework --queue $Queue --build-number $BuildNumber --build-configs $Configurations"

if ($RunFromPerformanceRepo) {
    $SetupArguments = "--perf-hash $CommitSha $CommonSetupArguments"
    
    robocopy $SourceDirectory $PerformanceDirectory /E /XD $PayloadDirectory $SourceDirectory\artifacts $SourceDirectory\.git
}
else {
    $SetupArguments = "--repository https://github.com/$Repository --branch $Branch --get-perf-hash --commit-sha $CommitSha $CommonSetupArguments"
    
    git clone --branch master --depth 1 --quiet https://github.com/dotnet/performance $PerformanceDirectory
}

if ($UseCoreRun) {
    $NewCoreRoot = (Join-Path $PayloadDirectory "Core_Root")
    Move-Item -Path $CoreRootDirectory -Destination $NewCoreRoot
}

$DocsDir = (Join-Path $PerformanceDirectory "docs")
robocopy $DocsDir $WorkItemDirectory

# Set variables that we will need to have in future steps
$ci = $true

. "$PSScriptRoot\..\pipeline-logging-functions.ps1"

# Directories
Write-PipelineSetVariableCurrentJob -Name 'PayloadDirectory' -Value "$PayloadDirectory"
Write-PipelineSetVariableCurrentJob -Name 'PerformanceDirectory' -Value "$PerformanceDirectory"
Write-PipelineSetVariableCurrentJob -Name 'WorkItemDirectory' -Value "$WorkItemDirectory"

# Script Arguments
Write-PipelineSetVariableCurrentJob -Name 'Python' -Value "py -3"
Write-PipelineSetVariableCurrentJob -Name 'ExtraBenchmarkDotNetArguments' -Value "$ExtraBenchmarkDotNetArguments"
Write-PipelineSetVariableCurrentJob -Name 'SetupArguments' -Value "$SetupArguments"
Write-PipelineSetVariableCurrentJob -Name 'PerfLabArguments' -Value "$PerfLabArguments"
Write-PipelineSetVariableCurrentJob -Name 'BDNCategories' -Value "$RunCategories"
Write-PipelineSetVariableCurrentJob -Name 'TargetCsproj' -Value "$Csproj"
Write-PipelineSetVariableCurrentJob -Name 'Kind' -Value "$Kind"
Write-PipelineSetVariableCurrentJob -Name 'Architecture' -Value "$Architecture"
Write-PipelineSetVariableCurrentJob -Name 'UseCoreRun' -Value "$UseCoreRun"
Write-PipelineSetVariableCurrentJob -Name 'RunFromPerfRepo' -Value "$RunFromPerformanceRepo"

# Helix Arguments
Write-PipelineSetVariableCurrentJob -Name 'Creator' -Value "$Creator"
Write-PipelineSetVariableCurrentJob -Name 'Queue' -Value "$Queue"
Write-PipelineSetVariableCurrentJob -Name 'HelixSourcePrefix' -Value "$HelixSourcePrefix"
Write-PipelineSetVariableCurrentJob -Name '_BuildConfig' -Value "$Architecture.$Kind.$Framework"

exit 0