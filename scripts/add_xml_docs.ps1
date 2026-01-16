# Adds basic XML documentation comments to classes, methods, properties, and fields.
# This is a heuristic script; review changes after running.
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
  $path = $_.FullName
  $lines = Get-Content -LiteralPath $path
  $out = New-Object System.Collections.Generic.List[System.String]

  # helper to get previous non-empty non-comment line index
  function Get-PrevNonEmptyIndex([int]$idx) {
    for ($k=$idx-1; $k -ge 0; $k--) {
      if ($lines[$k].Trim() -ne '') { return $k }
    }
    return -1
  }

  # process lines with insertion
  for ($i=0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    $trim = $line.Trim()

    # Skip if this is already a doc comment
    if ($trim.StartsWith('///')) { $out.Add($line); continue }

    # Patterns
    $classMatch = [regex]::Match($trim, '^(public|internal|protected|private|static|partial|sealed|abstract|\s)*\s*(class|interface|struct|enum)\s+([A-Za-z_][\w<>]*)')
    $methodMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|virtual|override|async|sealed|partial|extern|unsafe|\s)+\s+([A-Za-z0-9_<>,\[\] ]+)\s+([A-Za-z_][\w]*)\s*\((.*)\)')
    $propertyMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|virtual|override|abstract|sealed|readonly|\s)+\s+([A-Za-z0-9_<>,\[\] ]+)\s+([A-Za-z_][\w]*)\s*{')
    $fieldMatch = [regex]::Match($trim, '^(?:public|internal|protected|private|static|const|readonly|volatile|\s)+\s+([A-Za-z0-9_<>,\[\] ]+)\s+([A-Za-z_][\w]*)\s*(=|;)')
    $ctorMatch = [regex]::Match($trim, '^(?:public|internal|protected|private)\s+([A-Za-z_][\w]*)\s*\((.*)\)')

    if ($classMatch.Success) {
      # find insert index before attributes
      $j = $i
      while ($j -gt 0 -and $lines[$j-1].Trim().StartsWith('[')) { $j-- }
      # skip if previous non-empty line already has doc comment
      $prev = -1
      for ($k=$j-1; $k -ge 0; $k--) { if ($lines[$k].Trim() -ne '') { $prev = $k; break } }
      if ($prev -ge 0 -and $lines[$prev].Trim().StartsWith('///')) { $out.Add($line); continue }

      $indent = ($line -replace '^([ \t]*).*', '$1')
      $kind = $classMatch.Groups[2].Value
      $name = $classMatch.Groups[3].Value
      $out.Add($indent + '/// <summary>')
      $out.Add($indent + "/// Represents the $name $kind.")
      $out.Add($indent + '/// </summary>')
      $out.Add($line)
      continue
    }

    if ($methodMatch.Success -and $trim -notmatch '^if\s*\(' -and $trim -notmatch '^for\s*\(' -and $trim -notmatch '^foreach\s*\(' -and $trim -notmatch '^while\s*\(') {
      # handle constructors: name equals class name. We'll check later; but get params
      $ret = $methodMatch.Groups[1].Value.Trim()
      $mname = $methodMatch.Groups[2].Value
      $params = $methodMatch.Groups[3].Value.Trim()

      # find insert index before attributes
      $j = $i
      while ($j -gt 0 -and $lines[$j-1].Trim().StartsWith('[')) { $j-- }
      $prev = -1
      for ($k=$j-1; $k -ge 0; $k--) { if ($lines[$k].Trim() -ne '') { $prev = $k; break } }
      if ($prev -ge 0 -and $lines[$prev].Trim().StartsWith('///')) { $out.Add($line); continue }

      $indent = ($line -replace '^([ \t]*).*', '$1')
      $out.Add($indent + '/// <summary>')
      $out.Add($indent + "/// Executes the $mname method.")
      $out.Add($indent + '/// </summary>')

      # params
      if ($params -ne '') {
        $paramParts = [regex]::Split($params, ',') | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
        foreach ($p in $paramParts) {
          # param may be 'int x' or 'string[] args' or 'CancellationToken ct = default'
          $pname = ($p -split '\s+' )[-1] -replace '=.*$',''
          $out.Add($indent + '/// <param name="' + $pname + '"></param>')
        }
      }

      if ($ret -ne 'void') { $out.Add("$indent/// <returns></returns>") }

      $out.Add($line)
      continue
    }

    if ($propertyMatch.Success) {
      $j = $i
      while ($j -gt 0 -and $lines[$j-1].Trim().StartsWith('[')) { $j-- }
      $prev = -1
      for ($k=$j-1; $k -ge 0; $k--) { if ($lines[$k].Trim() -ne '') { $prev = $k; break } }
      if ($prev -ge 0 -and $lines[$prev].Trim().StartsWith('///')) { $out.Add($line); continue }

      $indent = ($line -replace '^([ \t]*).*', '$1')
      $pname = $propertyMatch.Groups[2].Value
      $out.Add($indent + '/// <summary>')
      $out.Add($indent + "/// Gets or sets the $pname.")
      $out.Add($indent + '/// </summary>')
      $out.Add($line)
      continue
    }

    if ($fieldMatch.Success) {
      $j = $i
      while ($j -gt 0 -and $lines[$j-1].Trim().StartsWith('[')) { $j-- }
      $prev = -1
      for ($k=$j-1; $k -ge 0; $k--) { if ($lines[$k].Trim() -ne '') { $prev = $k; break } }
      if ($prev -ge 0 -and $lines[$prev].Trim().StartsWith('///')) { $out.Add($line); continue }

      $indent = ($line -replace '^([ \t]*).*', '$1')
      $fname = $fieldMatch.Groups[2].Value
      $out.Add($indent + '/// <summary>')
      $out.Add($indent + "/// The $fname field.")
      $out.Add($indent + '/// </summary>')
      $out.Add($line)
      continue
    }

    # default: copy line
    $out.Add($line)
  }

  # write out if changed
  $new = $out -join "`n"
  $orig = $lines -join "`n"
  if ($new -ne $orig) {
    Copy-Item -Path $path -Destination ($path + ".bak") -Force
    Set-Content -LiteralPath $path -Value $new -Encoding UTF8
    Write-Host "Annotated: $path"
  }
}
