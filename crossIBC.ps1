param(
    [Parameter(Mandatory=$true)][string]$ToolSource,
    [Parameter(Mandatory=$true)][string]$ToolUser,
    [Parameter(Mandatory=$true)][string]$ToolPAT,
    [bool]$UsePartialNgen
)

function Find-IBCMerge {
    $IBCMergePath = gci -recurse .\.packages | where {$_.Name.Contains("ibcmerge.exe")}
    if(!$IBCMergePath)
    {
        $IBCMergePath = gci -recurse C:\microsoft.dotnet.ibcmerge | where {$_.Name.Contains("ibcmerge.exe")}
    }
    if(!$IBCMergePath)
    {
        Write-Error -Category ResourceUnavailable "Could not find IBCMerge.exe"
        exit
    }
    $IBCMergePath.FullName
}

function Preprocess-IBCMerge
{
    foreach($file in gci -recurse .packages\optimization.linux-x64.ibc.corefx\*.dll)
    {
        & $IBCMerge -q -f -delete -mo $file.FullName $file.FullName.Replace(".dll", ".ibc")
        & $IBCMerge -mi $file.FullName
    }
}

function Apply-IBCData
{
    $count = 0
    $pwd = (Get-Item -Path ".\").FullName
    foreach($file in gci -recurse .packages\optimization.linux-x64.ibc.corefx\*.dll)
    {
        $fileToOptimize = gci -recurse Linux\*.dll | where {$_.Name.Contains($file.Name)} | where {$_.FullName.Contains("netcoreapp")}
        if($fileToOptimize)
        {
            if(!$UsePartialNgen)
            {
                $count = $count + 1
                Write-Host "$IBCMerge -q -f -mo $fileToOptimize -incremental $file"
                & $IBCMerge -q -f -mo $fileToOptimize -incremental $file
            }
            else
            {
                & $IBCMerge -q -f -mo $fileToOptimize -incremental $file -partialNGEN -minify
            }
            $copyLocation = Join-Path -Path $pwd -ChildPath "Optimized"
            Write-Host "Copy-Item $fileToOptimize -Destination $copyLocation"
            Copy-Item $fileToOptimize -Destination $copyLocation
        }
    }
    echo $count
}

.\build.cmd -restore /p:OptionalToolSource=$ToolSource /p:OptionalToolSourceUser=$ToolUser /p:OptionalToolSourcePassword=$ToolPAT /p:EnableProfileGuidedOptimization=true /p:IBCTarget=Linux -release -ci
mkdir Optimized
$IBCMerge = Find-IBCMerge
Preprocess-IBCMerge
Apply-IBCData
