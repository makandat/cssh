# Validates generated CLASSES.md for common issues
$md = Get-Content -LiteralPath (Join-Path (Get-Location) 'CLASSES.md') -ErrorAction Stop
$bad = @()
for ($i=0; $i -lt $md.Length; $i++) {
  $line = $md[$i]
  if ($line -match '^- \*\*') {
    if ($line -match '\$sig') { $bad += "Line $($i+1): contains literal '$sig'" }
    if ($line -match '—\s*True$') { $bad += "Line $($i+1): summary is literal 'True'" }
    if ($line -match '—\s*False$') { $bad += "Line $($i+1): summary is literal 'False'" }
  }
}
if ($bad.Count -gt 0) {
  Write-Host "Validation failed: found issues in CLASSES.md:" -ForegroundColor Red
  $bad | ForEach-Object { Write-Host " - $_" }
  exit 1
}
Write-Host "Validation OK" -ForegroundColor Green
exit 0
