# Generates CLASSES.md from /// XML comments in C# source files
# Skips bin/, obj/, and scripts/ folders
$classes = @{}

Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' } | ForEach-Object {
  $path = $_.FullName
  $lines = Get-Content -LiteralPath $path

  $namespace = ''
  $classStack = @() # stack of @{name, fullname, startDepth}
  $braceDepth = 0
  $pendingDoc = @()

  for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    $trim = $line.Trim()

    if ($trim -match '^namespace\s+(.+)') {
      $namespace = $matches[1].Trim()
    }

    if ($trim -match '^///') {
      # strip leading '///' and single space
      $pendingDoc += ($trim -replace '^///\s?','')
      continue
    }

    # class/interface/struct/enum
    $classMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|partial|sealed|abstract|\s)*\s*(class|interface|struct|enum)\s+([A-Za-z_][\w]*)')
    if ($classMatch.Success) {
      $kind = $classMatch.Groups[1].Value
      $name = $classMatch.Groups[2].Value
      $fullname = if ($namespace -ne '') { "$namespace.$name" } else { $name }

      # extract summary from pendingDoc
      $summary = ''
      if ($pendingDoc.Count -gt 0) {
        $summaryLines = @()
        $inSummary = $false
        foreach ($d in $pendingDoc) {
          if ($d -match '<summary>') { $inSummary = $true; $d = $d -replace '.*<summary>','' }
          if ($d -match '</summary>') { $inSummary = $false; $d = $d -replace '</summary>.*','' }
          if (-not ($d -match '^<.*>$')) {
            $summaryLines += $d.Trim()
          }
        }
        $summary = ($summaryLines -join ' ') -replace '\s+',' ' -replace '^\s+|\s+$',''
      }

      if (-not $classes.ContainsKey($fullname)) {
        $classes[$fullname] = @{ Name = $name; FullName = $fullname; Kind = $kind; Summary = $summary; Members = @(); Source = $path }
      }

      $classStack += @{ Name = $name; FullName = $fullname; StartDepth = $braceDepth }
      $pendingDoc = @()
    }

    # if inside a class, detect members
    if ($classStack.Count -gt 0) {
      $currentClass = $classStack[-1]
      # constructor
      $ctorPattern = '^(?:public|internal|protected|private)\s+' + [regex]::Escape($currentClass.Name) + '\s*\((.*)\)'
      if ($trim -match $ctorPattern) {
        $sig = $trim -replace '\s*{\s*$',''
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Constructor'; Signature=$sig; Summary=$summary }
        $pendingDoc = @()
      }

      # method
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

      # property
      $propMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|virtual|override|abstract|sealed|readonly|\s)+\s+([A-Za-z0-9_<>,\[\]\s]+)\s+([A-Za-z_][\w]*)\s*{')
      if ($propMatch.Success) {
        $ptype = $propMatch.Groups[1].Value.Trim()
        $pname = $propMatch.Groups[2].Value
        $summary = ($pendingDoc | Where-Object { $_ -notmatch '^<.*' } ) -join ' '
        $summary = $summary -replace '\s+',' ' -replace '^\s+|\s+$',''
        $classes[$currentClass.FullName].Members += @{ Kind='Property'; Name=$pname; Type=$ptype; Signature=$trim; Summary=$summary }
        $pendingDoc = @()
      }

      # field
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

    # update brace depth
    $openCount = ($line -split '{').Length - 1
    $closeCount = ($line -split '}').Length - 1
    $braceDepth += $openCount - $closeCount

    # pop class stack if scope ended
    while ($classStack.Count -gt 0 -and $braceDepth -lt $classStack[-1].StartDepth) {
      $null = $classStack.RemoveAt($classStack.Count - 1)
    }

    # clear pendingDoc if non-doc non-empty line and not immediately preceding declaration
    if ($trim -ne '' -and -not $trim.StartsWith('///')) {
      if ($pendingDoc.Count -gt 1000) { $pendingDoc = @() }
    }
  }
}

# Build markdown
$md = New-Object System.Text.StringBuilder
$md.AppendLine('# Classes') | Out-Null
$md.AppendLine('') | Out-Null
foreach ($key in ($classes.Keys | Sort-Object)) {
  $c = $classes[$key]
  $md.AppendLine("## $($c.FullName)") | Out-Null
  if ($c.Summary -ne '') { $md.AppendLine("$($c.Summary)") | Out-Null }
  $md.AppendLine('') | Out-Null
  $md.AppendLine("**Source:** $($c.Source)") | Out-Null
  $md.AppendLine('') | Out-Null
  if ($c.Members.Count -gt 0) {
    $md.AppendLine('### Members') | Out-Null
    $md.AppendLine('') | Out-Null
    foreach ($m in $c.Members) {
      $kind = $m.Kind
      # ensure signature string with sensible fallback
      $sig = ''
      if ($m.ContainsKey('Signature') -and $m.Signature) { $sig = [string]$m.Signature }
      else {
        if ($m.ContainsKey('Name') -and $m.Name) {
          if ($m.Kind -eq 'Property' -and $m.ContainsKey('Type')) { $sig = "$($m.Type) $($m.Name)" }
          elseif ($m.Kind -eq 'Field' -and $m.ContainsKey('Type')) { $sig = "$($m.Type) $($m.Name)" }
          else { $sig = $m.Name }
        }
      }
      if (-not $sig) { $sig = '(signature unavailable)' }
      $summary = ($m.Summary -as [string])
      if (-not $summary) { $summary = '' }

      switch ($kind) {
        'Method' { $md.AppendLine('- **Method** `' + ($sig) + '` — ' + $summary) | Out-Null }
        'Constructor' { $md.AppendLine('- **Constructor** `' + ($sig) + '` — ' + $summary) | Out-Null }
        'Property' { $md.AppendLine('- **Property** `' + ($sig) + '` — ' + $summary) | Out-Null }
        'Field' { $md.AppendLine('- **Field** `' + ($sig) + '` — ' + $summary) | Out-Null }
        default { $md.AppendLine('- `' + ($sig) + '` — ' + $summary) | Out-Null }
      }
    }
    $md.AppendLine('') | Out-Null
  }
  else {
    $md.AppendLine('_No documented members_') | Out-Null
    $md.AppendLine('') | Out-Null
  }
}

$mdFile = Join-Path -Path (Get-Location) -ChildPath 'CLASSES.md'
Set-Content -LiteralPath $mdFile -Value $md.ToString() -Encoding UTF8
Write-Host "Generated: $mdFile"