Write-Host "Running..."
$currentPath = Get-Location
$resouces = New-Object 'System.Collections.Generic.Dictionary[String,Collections.Generic.List[string]]'
foreach ($resourceFile in Get-ChildItem $currentPath  -recurse -include Strings.resx)
{
    if ($resourceFile -like "*\tests\*")
    {
        continue
    }

    #Write-Host "Analyzing  $($resourceFile)"

    [xml]$XDocument = Get-Content -Path $resourceFile    
    foreach($resource in $XDocument.SelectNodes("//root/data"))
    {
        if(!$resouces.ContainsKey($resource.name))
        {
            $resourceList = New-Object Collections.Generic.List[string]
            $resouces.Add($resource.name,$resourceList)
        }        
        $resouces[$resource.name].Add($resource.value);
    }                       
}

$duplicates = New-Object 'Collections.Generic.List[string]'

foreach($resouce in $resouces.GetEnumerator())
{
    $count = ($resouce.value | Get-Unique).count
    if($count -gt 1)
    {       
        foreach($value in $resouce.value.GetEnumerator())
        {
            $duplicates.Add("$($resouce.key) $($value)")
        }
    }
}
     
if($duplicates.Count -gt 0)
{
    foreach($dup in $duplicates.GetEnumerator())
    {
        Write-Host $($dup)
    }
}
else
{
    Write-Host "No duplicates found."
}            
   
