# Reuse parsing logic from generate_classes_md.ps1 but print member summaries and types for inspection
$classes = @{}
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' } | ForEach-Object {
  $path = $_.FullName
  $lines = Get-Content -LiteralPath $path

  $namespace = ''
  $classStack = @()
  $braceDepth = 0
  $pendingDoc = @()

  for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    $trim = $line.Trim()

    if ($trim -match '^namespace\s+(.+)') { $namespace = $matches[1].Trim() }

    if ($trim -match '^///') { $pendingDoc += ($trim -replace '^///\s?',''); continue }

    $classMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|partial|sealed|abstract|\s)*\s*(class|interface|struct|enum)\s+([A-Za-z_][\w]*)')
    if ($classMatch.Success) {
      $kind = $classMatch.Groups[1].Value
      $name = $classMatch.Groups[2].Value
      $fullname = if ($namespace -ne '') { "$namespace.$name" } else { $name }

      $summary = ''
      if ($pendingDoc.Count -gt 0) {
        $summaryLines = @()
        $inSummary = $false
        foreach ($d in $pendingDoc) {
          if ($d -match '<summary>') { $inSummary = $true; $d = $d -replace '.*<summary>','' }
          if ($d -match '</summary>') { $inSummary = $false; $d = $d -replace '</summary>.*','' }
          if (-not ($d -match '^<.*>$')) { $summaryLines += $d.Trim() }
        }
        $summary = ($summaryLines -join ' ') -replace '\s+',' ' -replace '^\s+|\s+$',''
      }

      if (-not $classes.ContainsKey($fullname)) {
        $classes[$fullname] = @{ Name = $name; FullName = $fullname; Kind = $kind; Summary = $summary; Members = @(); Source = $path }
      }

      $classStack += @{ Name = $name; FullName = $fullname; StartDepth = $braceDepth }
      $pendingDoc = @()
    }

    if ($classStack.Count -gt 0) {
      $currentClass = $classStack[-1]
      $ctorPattern = '^(?:public|internal|protected|private)\s+' + [regex]::Escape($currentClass.Name) + '\s*\((.*)\)'
      if ($trim -match $ctorPattern) {
        $sig = $trim -replace '\s*{\s*$',''
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Constructor'; Signature=$sig; Summary=$summary }
        $pendingDoc = @()
      }

      $methodMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|virtual|override|async|sealed|partial|extern|unsafe|\s)+\s+([A-Za-z0-9_<>,\[\]\s]+)\s+([A-Za-z_][\w]*)\s*\((.*)\)')
      if ($methodMatch.Success) {
        $ret = $methodMatch.Groups[1].Value.Trim()
        $mname = $methodMatch.Groups[2].Value
        $params = $methodMatch.Groups[3].Value.Trim()
        $sig = $trim -replace '\s*{\s*$',''
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Method'; Name=$mname; Return=$ret; Signature=$sig; Summary=$summary }
        $pendingDoc = @()
      }

      $propMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|virtual|override|abstract|sealed|readonly|\s)+\s+([A-Za-z0-9_<>,\[\]\s]+)\s+([A-Za-z_][\w]*)\s*{')
      if ($propMatch.Success) {
        $ptype = $propMatch.Groups[1].Value.Trim()
        $pname = $propMatch.Groups[2].Value
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Property'; Name=$pname; Type=$ptype; Signature=$trim; Summary=$summary }
        $pendingDoc = @()
      }

      $fieldMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|const|readonly|volatile|\s)+\s+([A-Za-z0-9_<>,\[\]\s]+)\s+([A-Za-z_][\w]*)\s*(=|;)')
      if ($fieldMatch.Success) {
        $ftype = $fieldMatch.Groups[1].Value.Trim()
        $fname = $fieldMatch.Groups[2].Value
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Field'; Name=$fname; Type=$ftype; Signature=$trim; Summary=$summary }
        $pendingDoc = @()
      }
    }

    $openCount = ($line -split '{').Length - 1
    $closeCount = ($line -split '}').Length - 1
    $braceDepth += $openCount - $closeCount

    while ($classStack.Count -gt 0 -and $braceDepth -lt $classStack[-1].StartDepth) {
      $null = $classStack.RemoveAt($classStack.Count - 1)
    }

    if ($trim -ne '' -and -not $trim.StartsWith('///')) {
      if ($pendingDoc.Count -gt 1000) { $pendingDoc = @() }
    }
  }
}

# Now inspect some entries
foreach ($key in ($classes.Keys | Sort-Object)) {
  $c = $classes[$key]
  Write-Host "Class: $($c.FullName) (Summary: '$($c.Summary)')"
  foreach ($m in $c.Members) {
    $s = $m.Summary
    Write-Host " - Member kind=$($m.Kind), signature='$($m.Signature)', summary=[$s] (type: $($s.GetType().Name))"
  }
  Write-Host ''
}
