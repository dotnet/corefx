Write-Host "Running..."
$currentPath = Get-Location
$resouces = New-Object 'System.Collections.Generic.Dictionary[String,Collections.Generic.List[ResouceRecord]]'
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
            $resourceList = New-Object Collections.Generic.List[ResouceRecord]            
            $resouces.Add($resource.name,$resourceList)
        }    
            
        $record = New-Object ResouceRecord
        $record.value = $resource.value
        $record.fileName = $resourceFile

        $resouces[$resource.name].Add($record);
    }                       
}

$duplicates = New-Object 'Collections.Generic.List[string]'

foreach($resouce in $resouces.GetEnumerator())
{
    $values = New-Object Collections.Generic.List[string]       

    foreach($value in $resouce.Value)
    {
        $values.Add($value.value);        
    }

    $count = ($values | Get-Unique).count

    if ($count -gt 1)
    {
         foreach($value in $resouce.value.GetEnumerator())
        {
            $duplicates.Add("Name: '$($resouce.key)' value: '$($value.value)' relative path: '$($value.fileName.Replace($currentPath,[string]::Empty))'")
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
   
class ResouceRecord
{
    [String]$value
    [String]$fileName
}