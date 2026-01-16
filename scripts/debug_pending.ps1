$path = 'd:\workspace\Cssh\cssh.Tests\AliasAndHistoryTests.cs'
$lines = Get-Content -LiteralPath $path
$pending = @()
for ($i = 0; $i -lt $lines.Length; $i++) {
  $line = $lines[$i]
  $trim = $line.Trim()
  Write-Host ("Line {0}: '{1}'" -f $i, $trim)
  if ($trim -match '^///') { $pending += ($trim -replace '^///\s?',''); Write-Host ("  appended pending: '{0}'" -f ($trim -replace '^///\s?','')); continue }
  $classMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|partial|sealed|abstract|\s)*\s*(class|interface|struct|enum)\s+([A-Za-z_][\w]*)')
  if ($classMatch.Success) { Write-Host "  class detected: $($classMatch.Groups[2].Value)"; $pending=@(); continue }
  $fieldMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|const|readonly|volatile|\s)+\s+([A-Za-z0-9_<>,\[\]\s]+)\s+([A-Za-z_][\w]*)\s*(=|;)')
  if ($fieldMatch.Success) {
    Write-Host "  field detected: $($fieldMatch.Groups[2].Value)"
    $summary = ($pending | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
    Write-Host 'PendingDoc contents:'
    foreach ($p in $pending) { Write-Host " - '$p'" }
    Write-Host "Computed summary: '$summary'"
    break
  }
}
