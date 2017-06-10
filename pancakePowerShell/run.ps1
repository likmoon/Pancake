$json = Get-Content $triggerInput | ConvertFrom-Json
Write-Output "PowerShell script processed queue message '$json'"

$entity = [PSObject]@{
  PartitionKey = $json.PartitionKey
  RowKey = $json.Rowkey
  Title = $json.title
  ValidFrom = $json.ValidFrom
}

$entity | ConvertTo-Json | Out-File -Encoding UTF8 $outputTable